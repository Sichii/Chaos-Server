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
    /// <param name="gridDetails">
    ///     Details of a pathfinding grid
    /// </param>
    public Pathfinder(IGridDetails gridDetails)
    {
        Width = gridDetails.Width;
        Height = gridDetails.Height;
        PathNodes = new PathNode[Width, Height];
        PriortyQueue = new PriorityQueue<PathNode, int>(byte.MaxValue);

        NeighborIndexes = Enumerable.Range(0, 4)
                                    .Shuffle()
                                    .ToArray();
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

    /// <summary>
    ///     Finds a path from start to end, returning the next direction to walk to get there
    /// </summary>
    /// <param name="start">
    ///     The starting point
    /// </param>
    /// <param name="end">
    ///     Where to find a path to
    /// </param>
    /// <param name="ignoreWalls">
    ///     Whether or not to ignore walls
    /// </param>
    /// <param name="ignoreBlockingReactors">
    /// </param>
    /// <param name="blocked">
    ///     A collection of extra unwalkable points such as creatures
    /// </param>
    /// <param name="limitRadius">
    ///     Specify a max radius to use for path calculation, this can help with performance by limiting node discovery
    /// </param>
    public Stack<IPoint> FindPath(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked,
        int? limitRadius = null)
    {
        //if we're standing on the end already
        //try to walk out from under it
        if (PointEqualityComparer.Instance.Equals(start, end))
        {
            var randomPoint = FindRandomPoint(
                start,
                ignoreWalls,
                ignoreBlockingReactors,
                blocked);

            if (randomPoint.HasValue)
                return new Stack<IPoint>([randomPoint.Value]);

            return new Stack<IPoint>();
        }

        if (start.DistanceFrom(end) == 0)
            return new Stack<IPoint>();

        using var @lock = Sync.Enter();

        var blockedPoints = blocked.ToList();
        List<Point>? subGrid = null;

        if (limitRadius.HasValue)
            subGrid = start.SpiralSearch(limitRadius.Value)
                           .ToList();

        InitializeGrid(blockedPoints, subGrid);

        var path = InnerFindPath(
            start,
            end,
            ignoreWalls,
            ignoreBlockingReactors);

        PriortyQueue.Clear();

        ResetGrid(blockedPoints, subGrid);

        if (path.Count == 0)
        {
            var nextDirection = FindSimpleDirection(
                start,
                end,
                ignoreWalls,
                ignoreBlockingReactors,
                blocked);

            var nextPoint = start.DirectionalOffset(nextDirection);

            path.Push(nextPoint);
        }

        return path;
    }

    /// <inheritdoc />
    public Direction FindRandomDirection(
        IPoint start,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked)
    {
        var optimalPoint = FindRandomPoint(
            start,
            ignoreWalls,
            ignoreBlockingReactors,
            blocked);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.Value.DirectionalRelationTo(start);
    }

    /// <inheritdoc />
    public Direction FindSimpleDirection(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked)
    {
        var directionBias = end.DirectionalRelationTo(start);

        var points = start.GenerateCardinalPoints()
                          .Shuffle()
                          .WithConsistentDirectionBias(directionBias);

        var optimalPoint = GetFirstWalkablePoint(
            points,
            ignoreWalls,
            ignoreBlockingReactors,
            blocked);

        if (!optimalPoint.HasValue)
            return Direction.Invalid;

        return optimalPoint.Value.DirectionalRelationTo(start);
    }

    private Point? FindRandomPoint(
        IPoint start,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked)
        => GetFirstWalkablePoint(
            start.GenerateCardinalPoints()
                 .Shuffle(),
            ignoreWalls,
            ignoreBlockingReactors,
            blocked);

    private Point? GetFirstWalkablePoint(
        IEnumerable<Point> points,
        bool ignoreWalls,
        bool ignoreBlockingReactors,
        IReadOnlyCollection<IPoint> blocked)
        => points.FirstOrDefault(
            point => WithinGrid(point)
                     && PathNodes[point.X, point.Y]
                         .IsWalkable(ignoreWalls, ignoreBlockingReactors)
                     && !blocked.Contains(point, PointEqualityComparer.Instance));

    private IEnumerable<IPoint> GetParentChain(PathNode pathNode)
    {
        while (pathNode.Parent != null)
        {
            yield return pathNode;

            pathNode = pathNode.Parent;
        }
    }

    private void InitializeGrid(IEnumerable<IPoint> blocked, IEnumerable<Point>? subGrid = null)
    {
        //un-close all the nodes in the sub grid
        //the sub grid is the path-searchable area
        //optimization to avoid boxing points
        if (subGrid is not null)
        {
            foreach (var point in subGrid)
                if (WithinGrid(point))
                {
                    var node = PathNodes[point.X, point.Y];

                    if (!node.IsBlackListed)
                        PathNodes[point.X, point.Y].Closed = false;
                }
        } else
            foreach (var point in PathNodes.Flatten())
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

    private Stack<IPoint> InnerFindPath(
        IPoint start,
        IPoint end,
        bool ignoreWalls,
        bool ignoreBlockingReactors)
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

            //for each undiscovered walkable neighbor, set its parent and add it to the queue
            for (var i = 0; i < 4; i++)
            {
                //get a random neighbor
                var neighbor = node.Neighbors[NeighborIndexes[i]];

                //if it's a blocking reactor, and we haven't chosen to ignore them, skip it
                if ((neighbor == null) || neighbor.Closed || neighbor.Open || (!ignoreBlockingReactors && neighbor.IsBlockingReactor))
                    continue;

                //if we locate the end, set parent and break out
                //we're ok with this even if the end is inside a wall
                if (neighbor.Equals(end))
                {
                    neighbor.Parent = node;

                    return new Stack<IPoint>(GetParentChain(endNode));
                }

                //don't re-add nodes we've already considered
                //don't add walls unless ignoring walls
                //don't add blocked nodes
                //we can only ignore blocking reactors if it's the last point in the path
                if (!neighbor.IsWalkable(ignoreWalls, false))
                    continue;

                neighbor.Parent = node;
                PriortyQueue.Enqueue(neighbor, neighbor.DistanceFrom(start) + neighbor.DistanceFrom(end));
                neighbor.Open = true;
            }

            node.Closed = true;
            node.Open = false;
        }

        return new Stack<IPoint>(GetParentChain(endNode));
    }

    private void ResetGrid(IEnumerable<IPoint> blocked, IEnumerable<Point>? subGrid = null)
    {
        //optimization to avoid boxing points
        if (subGrid is not null)
        {
            foreach (var point in subGrid)
                if (WithinGrid(point))
                    PathNodes[point.X, point.Y]
                        .Reset();
        } else
            foreach (var point in PathNodes.Flatten())
                if (WithinGrid(point))
                    PathNodes[point.X, point.Y]
                        .Reset();

        //necessary in case a blocked point was specified outside the sub grid
        foreach (var point in blocked)
            if (WithinGrid(point))
                PathNodes[point.X, point.Y].IsBlocked = false;
    }

    private bool WithinGrid<TPoint>(TPoint point) where TPoint: IPoint
        => (point.X >= 0) && (point.X < Width) && (point.Y >= 0) && (point.Y < Height);
}