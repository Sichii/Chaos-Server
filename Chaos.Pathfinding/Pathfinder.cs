using Chaos.Core.Extensions;
using Chaos.Core.Synchronization;
using Chaos.Geometry;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.Extensions;
using Chaos.Geometry.Interfaces;
using Chaos.Pathfinding.Interfaces;

namespace Chaos.Pathfinding;

public class Pathfinder : IPathfinder
{
    private readonly int Height;
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
        Sync = new AutoReleasingMonitor();

        //create nodes, assign walls
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                PathNodes[x, y] = new PathNode(x, y);

        foreach (var wall in gridDetails.Walls)
            PathNodes[wall.X, wall.Y].IsWall = true;

        //assign node neighbors
        foreach (var pathNode in PathNodes.Flatten())
            foreach (var point in pathNode.GetCardinalPoints())
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
        var subGrid = new Rectangle(end, 13, 13); //CalculateSubGrid(start, end);
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
        PriortyQueue.Enqueue(startNode, 0);
        var endNode = default(PathNode?);

        while ((PriortyQueue.Count > 0) && (endNode == null))
        {
            var node = PriortyQueue.Dequeue();

            if (node.Closed)
                continue;

            //for each undiscovered walkable neighbor, set it's parent and add it to the queue
            foreach (var neighbor in node.Neighbors.Shuffle())
            {
                if (neighbor == null)
                    continue;

                //if we locate the end, set parent and break out
                //we're ok with this even if the end is inside a wall
                if (neighbor.Equals(end))
                {
                    neighbor.Parent = node;
                    endNode = neighbor;

                    break;
                }

                //don't re-add nodes we've already considered
                //don't add walls
                // ReSharper disable once MergeIntoNegatedPattern
                if (neighbor.Closed || neighbor.Open || !neighbor.IsWalkable(ignoreWalls))
                    continue;

                neighbor.Parent = node;
                PriortyQueue.Enqueue(neighbor, neighbor.DistanceFrom(start) + neighbor.DistanceFrom(end));
                neighbor.Open = true;
            }

            node.Closed = true;
            node.Open = false;
        }

        if (endNode == null)
            return Enumerable.Empty<IPoint>();

        return TracePath(endNode);
    }

    private Direction FindSimpleDirectionOrInvalid(
        IPoint start,
        IPoint end,
        bool ignoreWalls
    )
    {
        var optimalPoint = GetFirstWalkablePoint(start.GetCardinalPoints().OrderBy(p => p.DistanceFrom(end)), ignoreWalls);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.DirectionalRelationTo(start);
    }

    private Point? GetFirstWalkablePoint(IEnumerable<Point> points, bool ignoreWalls)
    {
        foreach (var point in points)
            if (WithinGrid(point) && PathNodes[point.X, point.Y].IsWalkable(ignoreWalls))
                return point;

        return null;
    }

    private void InitializeSubGrid(IRectangle subGrid, IEnumerable<IPoint> creatures)
    {
        //un-close all the nodes in the sub grid
        //the sub grid is the path-searchable area
        foreach (var point in subGrid.Points())
            PathNodes[point.X, point.Y].Closed = false;

        foreach (var creature in creatures)
            PathNodes[creature.X, creature.Y].IsCreature = true;
    }

    public Direction Pathfind(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        IEnumerable<IPoint> creatures
    )
    {
        //if we're standing on the end already
        //try to walk out from under it
        if (Point.From(start).Equals(end))
            return FindSimpleDirectionOrInvalid(
                start,
                end,
                ignoreWalls);

        var nextPoint = FindOptimalPoint(
            start,
            end,
            ignoreWalls,
            creatures);

        //failed to find path
        //find a direction to walk(if any) via simple logic
        if (nextPoint == null)
            return FindSimpleDirectionOrInvalid(
                start,
                end,
                ignoreWalls);

        return nextPoint.DirectionalRelationTo(start);
    }

    private void ResetSubGrid(IRectangle subGrid, IEnumerable<IPoint> creatures)
    {
        foreach (var point in subGrid.Points())
            PathNodes[point.X, point.Y].Reset();

        foreach (var creature in creatures)
            PathNodes[creature.X, creature.Y].IsCreature = false;
    }

    private IEnumerable<IPoint> TracePath(PathNode pathNode)
    {
        IEnumerable<IPoint> InnerGetPath()
        {
            while (pathNode.Parent != null)
            {
                yield return pathNode;

                pathNode = pathNode.Parent;
            }
        }

        return InnerGetPath().Reverse();
    }

    /*
    private Rectangle CalculateSubGrid(IPoint start, IPoint end)
    {
        var left = Math.Max(Math.Min(start.X, end.X) - 13, 0);
        var right = Math.Min(Math.Max(start.X, end.X), Width - 1);
        var top = Math.Max(Math.Min(start.Y, end.Y) - 13, 0);
        var bottom = Math.Min(Math.Max(start.Y, end.Y), Height - 1);
        var width = right - left;
        var height = bottom - top;

        return new Rectangle
        {
            Left = left,
            Right = right,
            Top = top,
            Bottom = bottom,
            Width = width,
            Height = height
        };
    }*/

    private bool WithinGrid(IPoint point) => (point.X >= 0) && (point.X < Width) && (point.Y >= 0) && (point.Y < Height);
}