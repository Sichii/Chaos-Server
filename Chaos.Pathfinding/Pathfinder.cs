#region
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Pathfinding.Abstractions;
#endregion

namespace Chaos.Pathfinding;

/// <summary>
///     Provides a pathfinding implementation
/// </summary>
public sealed class Pathfinder : IPathfinder
{
    private readonly int Height;
    private readonly int[] NeighborIndexes;
    private readonly PathNode[,] PathNodes;
    private readonly PriorityQueue<PathNode, int> PriorityQueue;
    private readonly Lock Sync;
    private readonly int Width;

    /// <summary>
    ///     Creates a new instance of <see cref="Pathfinder" />
    /// </summary>
    /// <param name="gridDetails">
    ///     Details of a pathfinding grid
    /// </param>
    public Pathfinder(IGridDetails gridDetails)
    {
        Width = gridDetails.Width;
        Height = gridDetails.Height;
        PathNodes = new PathNode[Width, Height];
        PriorityQueue = new PriorityQueue<PathNode, int>(byte.MaxValue);

        NeighborIndexes = Enumerable.Range(0, 4)
                                    .Shuffle()
                                    .ToArray();
        Sync = new Lock();

        //create nodes, assign walls
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                PathNodes[x, y] = new PathNode(x, y);

        foreach (var wall in gridDetails.Walls)
            PathNodes[wall.X, wall.Y].IsWall = true;

        foreach (var blockingReactor in gridDetails.BlockingReactors)
            PathNodes[blockingReactor.X, blockingReactor.Y].IsBlockingReactor = true;

        //assign node neighbors
        foreach (var pathNode in PathNodes.Flatten())
            foreach (var point in pathNode.GenerateCardinalPoints())
                if (WithinGrid(point))
                {
                    var relation = (int)point.DirectionalRelationTo(pathNode);
                    pathNode.Neighbors[relation] = PathNodes[point.X, point.Y];
                }
    }

    /// <inheritdoc />
    public Direction FindOptimalDirection(IPoint start, IPoint end, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default;

        //if we're standing on the end already
        //try to walk out from under it
        if (PointEqualityComparer.Instance.Equals(start, end))
        {
            var randomPoint = FindRandomPoint(start, pathOptions);

            if (randomPoint.HasValue)
                return randomPoint.Value.DirectionalRelationTo(start);

            return Direction.Invalid;
        }

        if (start.ManhattanDistanceFrom(end) == 0)
            return Direction.Invalid;

        using var @lock = Sync.EnterScope();

        var blockedPoints = pathOptions.BlockedPoints;
        var area = CalculateSpiralSearchArea(pathOptions.LimitRadius ?? 0);
        Span<Point> subGrid = stackalloc Point[area];

        if (pathOptions.LimitRadius.HasValue)
        {
            var i = 0;

            foreach (var point in start.SpiralSearch(pathOptions.LimitRadius.Value))
                subGrid[i++] = point;
        } else
            subGrid = default;

        InitializeGrid(blockedPoints, subGrid);

        var optimalPoint = InnerFindPath(start, end, pathOptions)
            .LastOrDefault();

        PriorityQueue.Clear();

        ResetGrid(blockedPoints, subGrid);

        if (optimalPoint is null)
            return FindSimpleDirection(start, end, pathOptions);

        return optimalPoint.DirectionalRelationTo(start);
    }

    /// <summary>
    ///     Finds a path from start to end, returning the next direction to walk to get there
    /// </summary>
    /// <param name="start">
    ///     The starting point
    /// </param>
    /// <param name="end">
    ///     Where to find a path to
    /// </param>
    /// <param name="pathOptions">
    ///     Optional options to use for pathfinding
    /// </param>
    public Stack<IPoint> FindPath(IPoint start, IPoint end, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default;

        //if we're standing on the end already
        //try to walk out from under it
        if (PointEqualityComparer.Instance.Equals(start, end))
        {
            var randomPoint = FindRandomPoint(start, pathOptions);

            if (randomPoint.HasValue)
                return new Stack<IPoint>([randomPoint.Value]);

            return new Stack<IPoint>();
        }

        if (start.ManhattanDistanceFrom(end) == 0)
            return new Stack<IPoint>();

        using var @lock = Sync.EnterScope();

        var blockedPoints = pathOptions.BlockedPoints;

        //calculate the area the spiral search will occupy
        var area = CalculateSpiralSearchArea(pathOptions.LimitRadius ?? 0);

        //stackalloc for perf
        Span<Point> subGrid = stackalloc Point[area];

        if (pathOptions.LimitRadius.HasValue)
        {
            var i = 0;

            foreach (var point in start.SpiralSearch(pathOptions.LimitRadius.Value))
                subGrid[i++] = point;
        } else
            subGrid = default;

        InitializeGrid(blockedPoints, subGrid);

        var path = new Stack<IPoint>(InnerFindPath(start, end, pathOptions));

        PriorityQueue.Clear();

        ResetGrid(blockedPoints, subGrid);

        if (path.Count == 0)
        {
            var nextDirection = FindSimpleDirection(start, end, pathOptions);

            var nextPoint = start.DirectionalOffset(nextDirection);

            path.Push(nextPoint);
        }

        return path;
    }

    /// <inheritdoc />
    public Direction FindRandomDirection(IPoint start, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default;

        var optimalPoint = FindRandomPoint(start, pathOptions);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.Value.DirectionalRelationTo(start);
    }

    /// <inheritdoc />
    public Direction FindSimpleDirection(IPoint start, IPoint end, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default;

        var directionBias = end.DirectionalRelationTo(start);

        var points = start.GenerateCardinalPoints()
                          .WithConsistentDirectionBias(directionBias);

        var optimalPoint = GetFirstWalkablePoint(points, pathOptions);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.Value.DirectionalRelationTo(start);
    }

    /// <summary>
    ///     Calculates the number of points generated by a spiral search with the given radius
    /// </summary>
    public static int CalculateSpiralSearchArea(int radius) => 2 * radius * (radius + 1) + 1;

    private Point? FindRandomPoint(IPoint start, IPathOptions pathOptions)
        => GetFirstWalkablePoint(
            start.GenerateCardinalPoints()
                 .Shuffle(),
            pathOptions);

    private Point? GetFirstWalkablePoint(IEnumerable<Point> points, IPathOptions pathOptions)
        => points.FirstOrDefault(point => WithinGrid(point)
                                          && PathNodes[point.X, point.Y]
                                              .IsWalkable(pathOptions.IgnoreWalls, pathOptions.IgnoreBlockingReactors)
                                          && !pathOptions.BlockedPoints.Contains(point, PointEqualityComparer.Instance));

    private IEnumerable<IPoint> GetParentChain(PathNode pathNode)
    {
        while (pathNode.Parent != null)
        {
            yield return pathNode;

            pathNode = pathNode.Parent;
        }
    }

    private void InitializeGrid(IEnumerable<IPoint> blocked, ReadOnlySpan<Point> subGrid = default)
    {
        //un-close all the nodes in the sub grid
        //the sub grid is the path-searchable area
        //optimization to avoid boxing points
        if (!subGrid.IsEmpty)
        {
            foreach (var point in subGrid)
                if (WithinGrid(point))
                {
                    var node = PathNodes[point.X, point.Y];

                    node.Closed = node.IsBlackListed;
                }
        } else
            foreach (var point in PathNodes.Flatten())
            {
                var node = PathNodes[point.X, point.Y];

                node.Closed = node.IsBlackListed;
            }

        foreach (var point in blocked)
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].IsBlocked = true;
    }

    private IEnumerable<IPoint> InnerFindPath(IPoint start, IPoint end, IPathOptions pathOptions)
    {
        var startNode = PathNodes[start.X, start.Y];
        var endNode = PathNodes[end.X, end.Y];
        PriorityQueue.Enqueue(startNode, 0);

        while (PriorityQueue.Count > 0)
        {
            var node = PriorityQueue.Dequeue();

            if (node.Closed)
                continue;

            NeighborIndexes.ShuffleInPlace();

            //for each undiscovered walkable neighbor, set its parent and add it to the queue
            for (var i = 0; i < 4; i++)
            {
                //get a random neighbor
                var neighbor = node.Neighbors[NeighborIndexes[i]];

                //if it's a blocking reactor, and we haven't chosen to ignore them, skip it
                if ((neighbor == null)
                    || neighbor.Closed
                    || neighbor.Open
                    || (!pathOptions.IgnoreBlockingReactors && neighbor.IsBlockingReactor))
                    continue;

                //if we locate the end, set parent and break out
                //we're ok with this even if the end is inside a wall
                if (neighbor.Equals(end))
                {
                    endNode.Parent = node;

                    return GetParentChain(endNode);
                }

                //don't re-add nodes we've already considered
                //don't add walls unless ignoring walls
                //don't add blocked nodes
                //we can only ignore blocking reactors if it's the last point in the path
                if (!neighbor.IsWalkable(pathOptions.IgnoreWalls, false))
                    continue;

                neighbor.Parent = node;
                PriorityQueue.Enqueue(neighbor, neighbor.ManhattanDistanceFrom(start) + neighbor.ManhattanDistanceFrom(end));
                neighbor.Open = true;
            }

            node.Closed = true;
            node.Open = false;
        }

        return GetParentChain(endNode);
    }

    private void ResetGrid(IEnumerable<IPoint> blocked, ReadOnlySpan<Point> subGrid = default)
    {
        //optimization to avoid boxing points
        if (!subGrid.IsEmpty)
        {
            foreach (var point in subGrid)
                if (WithinGrid(point))
                    PathNodes[point.X, point.Y]
                        .Reset();
        } else
            foreach (var node in PathNodes.Flatten())
                if (WithinGrid(node))
                    node.Reset();

        //necessary in case a blocked point was specified outside the sub grid
        foreach (var point in blocked)
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].IsBlocked = false;
    }

    private bool WithinGrid<TPoint>(TPoint point) where TPoint: IPoint, allows ref struct
        => (point.X >= 0) && (point.X < Width) && (point.Y >= 0) && (point.Y < Height);
}