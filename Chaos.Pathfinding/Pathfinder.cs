using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Pathfinding;

public sealed class Pathfinder : IPathfinder
{
    private readonly int Height;
    private readonly int[] NeighborIndexes;
    private readonly PathNode[,] PathNodes;
    private readonly PriorityQueue<PathNode, int> PriortyQueue;
    private readonly AutoReleasingMonitor Sync;
    private readonly int Width;

    public Pathfinder(IGridDetails gridDetails)
    {
        Width = gridDetails.Width;
        Height = gridDetails.Height;
        PathNodes = new PathNode[Width, Height];
        PriortyQueue = new PriorityQueue<PathNode, int>(byte.MaxValue);
        NeighborIndexes = Enumerable.Range(0, 4).Shuffle().ToArray();
        Sync = new AutoReleasingMonitor();

        //create nodes, assign walls
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                PathNodes[x, y] = new PathNode(x, y);

        foreach (var wall in gridDetails.Walls)
            PathNodes[wall.X, wall.Y].IsWall = true;

        //assign node neighbors
        foreach (var pathNode in PathNodes.Flatten())
            foreach (var point in pathNode.GenerateCardinalPoints())
                if (WithinGrid(point))
                {
                    var relation = (int)point.DirectionalRelationTo(pathNode);
                    pathNode.Neighbors[relation] = PathNodes[point.X, point.Y];
                }
    }

    private IPoint? FindOptimalPoint(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IEnumerable<IPoint> creatures
    )
    {
        using var @lock = Sync.Enter();

        //the only points the truly matter are the ones around the target
        //imagine a monster is pathing to you...
        //if it paths outside of your view, it will no longer aggro you
        var subGrid = new Rectangle(end, 27, 27); //CalculateSubGrid(start, end);
        var creatureCollection = creatures.ToList();

        InitializeSubGrid(subGrid, creatureCollection);

        var path = FindPath(start, end, ignoreWalls);
        var ret = path.FirstOrDefault();

        PriortyQueue.Clear();
        ResetSubGrid(subGrid, creatureCollection);

        return ret;
    }

    private IEnumerable<IPoint> FindPath(IPoint start, IPoint end, bool ignoreWalls)
    {
        var startNode = PathNodes[start.X, start.Y];
        var endNode = PathNodes[end.X, end.Y];
        PriortyQueue.Enqueue(startNode, 0);

        while (PriortyQueue.Count > 0)
        {
            var node = PriortyQueue.Dequeue();

            if (node.Closed)
                continue;

            NeighborIndexes.ShuffleInPlace();

            //for each undiscovered walkable neighbor, set it's parent and add it to the queue
            for (var i = 0; i < 4; i++)
            {
                var neighbor = node.Neighbors[NeighborIndexes[i]];

                if ((neighbor == null) || neighbor.Closed || neighbor.Open)
                    continue;

                //if we locate the end, set parent and break out
                //we're ok with this even if the end is inside a wall
                if (neighbor.Equals(end))
                {
                    neighbor.Parent = node;

                    return TracePath(endNode);
                }

                //don't re-add nodes we've already considered
                //don't add walls
                // ReSharper disable once MergeIntoNegatedPattern
                if (!neighbor.IsWalkable(ignoreWalls))
                    continue;

                neighbor.Parent = node;
                PriortyQueue.Enqueue(neighbor, neighbor.DistanceFrom(start) + neighbor.DistanceFrom(end));
                neighbor.Open = true;
            }

            node.Closed = true;
            node.Open = false;
        }

        return TracePath(endNode);
    }

    private Direction FindSimpleDirectionOrInvalid(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> creatures
    )
    {
        var directionBias = end.DirectionalRelationTo(start);

        var points = start.GenerateCardinalPoints()
                          .Shuffle()
                          .WithDirectionBias(directionBias);

        var optimalPoint = GetFirstWalkablePoint(points, ignoreWalls, creatures);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.DirectionalRelationTo(start);
    }

    private Point? GetFirstWalkablePoint(IEnumerable<Point> points, bool ignoreWalls, IReadOnlyCollection<IPoint> unwalkablePoints) =>
        points.FirstOrDefault(
            point => WithinGrid(point)
                     && PathNodes[point.X, point.Y].IsWalkable(ignoreWalls)
                     && !unwalkablePoints.Contains(point, PointEqualityComparer.Instance));

    private IEnumerable<IPoint> GetParentChain(PathNode pathNode)
    {
        while (pathNode.Parent != null)
        {
            yield return pathNode;

            pathNode = pathNode.Parent;
        }
    }

    private void InitializeSubGrid(IRectangle subGrid, IEnumerable<IPoint> unwalkablePoints)
    {
        //un-close all the nodes in the sub grid
        //the sub grid is the path-searchable area
        foreach (var point in subGrid.Points())
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].Closed = false;

        foreach (var creature in unwalkablePoints)
            PathNodes[creature.X, creature.Y].IsCreature = true;
    }

    public Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> unwalkablePoints
    )
    {
        //if we're standing on the end already
        //try to walk out from under it
        if (PointEqualityComparer.Instance.Equals(start, end))
            return Wander(start, ignoreWalls, unwalkablePoints);

        if (start.DistanceFrom(end) == 1)
            return end.DirectionalRelationTo(start);

        var nextPoint = FindOptimalPoint(
            start,
            end,
            ignoreWalls,
            unwalkablePoints);

        //failed to find path
        //find a direction to walk(if any) via simple logic
        if (nextPoint == null)
            return FindSimpleDirectionOrInvalid(
                start,
                end,
                ignoreWalls,
                unwalkablePoints);

        return nextPoint.DirectionalRelationTo(start);
    }

    private void ResetSubGrid(IRectangle subGrid, IEnumerable<IPoint> creatures)
    {
        foreach (var point in subGrid.Points())
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].Reset();

        foreach (var creature in creatures)
            PathNodes[creature.X, creature.Y].IsCreature = false;
    }

    private IEnumerable<IPoint> TracePath(PathNode pathNode) => GetParentChain(pathNode).Reverse();

    /// <inheritdoc />
    public Direction Wander(IPoint start, bool ignoreWalls, IReadOnlyCollection<IPoint> unwalkablePoints)
    {
        var optimalPoint = GetFirstWalkablePoint(start.GenerateCardinalPoints().Shuffle(), ignoreWalls, unwalkablePoints);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.DirectionalRelationTo(start);
    }

    private bool WithinGrid(IPoint point) => (point.X >= 0) && (point.X < Width) && (point.Y >= 0) && (point.Y < Height);
}