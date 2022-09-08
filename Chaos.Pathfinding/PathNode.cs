using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding;

internal class PathNode : IEquatable<IPoint>, IPoint
{
    public bool Closed { get; set; }
    public bool IsCreature { get; set; }
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
    }

    public bool Equals(IPoint? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return (X == other.X)
               && (Y == other.Y);
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

    public bool IsWalkable(bool ignoreWalls) => !IsCreature && (ignoreWalls || !IsWall);

    public void Reset()
    {
        IsCreature = false;
        //by default nodes are not searchable
        Closed = true;
        Open = false;
        Parent = null;
    }
}