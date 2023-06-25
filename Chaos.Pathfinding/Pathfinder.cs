using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Pathfinding.Abstractions;

namespace Chaos.Pathfinding;

/// <summary>
///     Provides a pathfinding implementation
/// </summary>
public sealed class Pathfinder : IPathfinder
{
    private readonly int Height;
    private readonly int[] NeighborIndexes;
    private readonly PathNode[,] PathNodes;
    private readonly PriorityQueue<PathNode, int> PriortyQueue;
    private readonly AutoReleasingMonitor Sync;
    private readonly int Width;

    /// <summary>
    ///     Creates a new instance of <see cref="Pathfinder" />
    /// </summary>
    /// <param name="gridDetails">Details of a pathfinding grid</param>
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

        foreach (var point in gridDetails.Blacklist)
            PathNodes[point.X, point.Y].IsBlackListed = true;

        //assign node neighbors
        foreach (var pathNode in PathNodes.Flatten())
            foreach (var point in pathNode.GenerateCardinalPoints())
                if (WithinGrid(point))
                {
                    var relation = (int)point.DirectionalRelationTo(pathNode);
                    pathNode.Neighbors[relation] = PathNodes[point.X, point.Y];
                }
    }

    /// <summary>
    ///     Finds a path from start to end, returning the next direction to walk to get there
    /// </summary>
    /// <param name="start">The starting point</param>
    /// <param name="end">Where to find a path to</param>
    /// <param name="ignoreWalls">Whether or not to ignore walls</param>
    /// <param name="blocked">A collection of extra unwalkable points such as creatures</param>
    /// <param name="limitRadius">
    ///     Specify a max radius to use for path calculation, this can help with performance by limiting
    ///     node discovery
    /// </param>
    public Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IReadOnlyCollection<IPoint> blocked,
        int? limitRadius = null
    )
    {
        //if we're standing on the end already
        //try to walk out from under it
        if (PointEqualityComparer.Instance.Equals(start, end))
            return Wander(start, ignoreWalls, blocked);

        if (start.DistanceFrom(end) == 1)
            return end.DirectionalRelationTo(start);

        var nextPoint = FindOptimalPoint(
            start,
            end,
            ignoreWalls,
            blocked,
            limitRadius);

        //failed to find path
        //find a direction to walk(if any) via simple logic
        if (nextPoint == null)
            return FindSimpleDirectionOrInvalid(
                start,
                end,
                ignoreWalls,
                blocked);

        return nextPoint.DirectionalRelationTo(start);
    }

    /// <inheritdoc />
    public Direction Wander(IPoint start, bool ignoreWalls, IReadOnlyCollection<IPoint> blocked)
    {
        var optimalPoint = GetFirstWalkablePoint(start.GenerateCardinalPoints().Shuffle(), ignoreWalls, blocked);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.DirectionalRelationTo(start);
    }

    private IPoint? FindOptimalPoint(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IEnumerable<IPoint> blocked,
        int? limitRadius = null
    )
    {
        using var @lock = Sync.Enter();

        var blockedPoints = blocked.ToList();
        List<IPoint>? subGrid = null;

        if (limitRadius.HasValue)
            subGrid = start.SpiralSearch(limitRadius.Value)
                           .ToListCast<IPoint>();

        InitializeGrid(blockedPoints, subGrid);

        var path = FindPath(start, end, ignoreWalls);
        var ret = path.FirstOrDefault();

        PriortyQueue.Clear();

        ResetGrid(blockedPoints, subGrid);

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
                //get a random neighbor
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
                //don't add walls unless ignoring walls
                //dont add blocked nodes
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
        IReadOnlyCollection<IPoint> blocked
    )
    {
        var directionBias = end.DirectionalRelationTo(start);

        var points = start.GenerateCardinalPoints()
                          .Shuffle()
                          .WithDirectionBias(directionBias);

        var optimalPoint = GetFirstWalkablePoint(points, ignoreWalls, blocked);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.DirectionalRelationTo(start);
    }

    private Point? GetFirstWalkablePoint(IEnumerable<Point> points, bool ignoreWalls, IReadOnlyCollection<IPoint> blocked) =>
        points.FirstOrDefault(
            point => WithinGrid(point)
                     && PathNodes[point.X, point.Y].IsWalkable(ignoreWalls)
                     && !blocked.Contains(point, PointEqualityComparer.Instance));

    private IEnumerable<IPoint> GetParentChain(PathNode pathNode)
    {
        while (pathNode.Parent != null)
        {
            yield return pathNode;

            pathNode = pathNode.Parent;
        }
    }

    private void InitializeGrid(IEnumerable<IPoint> blocked, IEnumerable<IPoint>? subGrid = null)
    {
        //un-close all the nodes in the sub grid
        //the sub grid is the path-searchable area
        foreach (var point in subGrid ?? PathNodes.Flatten())
            if (WithinGrid(point))
            {
                var node = PathNodes[point.X, point.Y];

                if (!node.IsBlackListed)
                    PathNodes[point.X, point.Y].Closed = false;
            }

        foreach (var point in blocked)
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].IsBlocked = true;
    }

    private void ResetGrid(IEnumerable<IPoint> blocked, IEnumerable<IPoint>? subGrid = null)
    {
        foreach (var point in subGrid ?? PathNodes.Flatten())
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].Reset();

        //necessary incase a blocked point was specified outside the sub grid
        foreach (var point in blocked)
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].IsBlocked = false;
    }

    private IEnumerable<IPoint> TracePath(PathNode pathNode) => GetParentChain(pathNode).Reverse();

    private bool WithinGrid(IPoint point) => (point.X >= 0) && (point.X < Width) && (point.Y >= 0) && (point.Y < Height);
}