using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding;

internal sealed class PathNode : IEquatable<IPoint>, IPoint
{
    public bool Closed { get; set; }

    /// <summary>
    ///     The node is blacklisted. Blacklisted nodes are not to ever be opened, and you shouldn't walk onto them even if it's
    ///     the last point in the path
    /// </summary>
    public bool IsBlackListed { get; set; }

    /// <summary>
    ///     The node is blocked. Blocked nodes are opened, but cannot be ignored. Blocked nodes can be walked on if it's the
    ///     last point in the path.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    ///     The node is a wall. Walls are opened, and can be ignored depending on the pathfinding request
    /// </summary>
    public bool IsWall { get; set; }

    public bool Open { get; set; }
    public PathNode? Parent { get; set; }
    public PathNode?[] Neighbors { get; }
    public int X { get; }
    public int Y { get; }

    public PathNode(int x, int y)
    {
        X = x;
        Y = y;
        Neighbors = new PathNode?[4];
        Closed = true;
    }

    public bool Equals(IPoint? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (X == other.X) && (Y == other.Y);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return Equals((IPoint)obj);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public bool IsWalkable(bool ignoreWalls) => !IsBlocked && (ignoreWalls || !IsWall);

    public void Reset()
    {
        //by default nodes are not searchable
        Closed = true;
        Open = false;
        Parent = null;
        IsBlocked = false;
    }
}