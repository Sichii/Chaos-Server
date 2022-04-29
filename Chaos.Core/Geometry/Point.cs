using Chaos.Core.Definitions;

namespace Chaos.Core.Geometry;

public readonly struct Point : IEquatable<Point>
{
    public ushort X { get; init; }

    public ushort Y { get; init; }
    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static implicit operator Point(ValueTuple<int, int> tuple) => new((ushort)tuple.Item1, (ushort)tuple.Item2);
    public static bool operator !=(Point left, Point right) => !left.Equals(right);

    /// <summary>
    ///     Json & Master constructor for a structure representing a point within a map.
    /// </summary>
    private Point(ushort x, ushort y)
    {
        X = x;
        Y = y;
    }

    public void Desconstruct(out ushort x, out ushort y)
    {
        x = X;
        y = Y;
    }

    /// <summary>
    ///     Gets this point's distance from another point.
    /// </summary>
    public int Distance(Point pt) => Distance(pt.X, pt.Y);

    /// <summary>
    ///     Gets this point's distance from another point.
    /// </summary>
    public int Distance(ushort x, ushort y) => Math.Abs(x - X) + Math.Abs(y - Y);

    public bool Equals(Point other) =>
        (X == other.X)
        && (Y == other.Y);

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    ///     Returns a new point that has been offset in a given direction.
    /// </summary>
    public Point Offset(Direction direction, int degree = 1) =>
        direction switch
        {
            Direction.North => (X, Y - degree),
            Direction.East  => (X + degree, Y),
            Direction.South => (X, Y + degree),
            Direction.West  => (X - degree, Y),
            _               => throw new InvalidOperationException()
        };

    /// <summary>
    ///     Returns the directional relation between this point and another point.
    ///     The direction this point is from the given point.
    /// </summary>
    public Direction Relation(Point point)
    {
        var direction = Direction.Invalid;
        var degree = 0;

        if (Y < point.Y)
        {
            degree = point.Y - Y;
            direction = Direction.North;
        }

        if ((X > point.X) && (X - point.X > degree))
        {
            degree = X - point.X;
            direction = Direction.East;
        }

        if ((Y > point.Y) && (Y - point.Y > degree))
        {
            degree = Y - point.Y;
            direction = Direction.South;
        }

        if ((X < point.X) && (point.X - X > degree))
            direction = Direction.West;

        return direction;
    }

    public override string ToString() => $@"({X}, {Y})";

    /// <summary>
    ///     Attempts to parse a point from a given string.
    /// </summary>
    public static bool TryParse(string str, out Point point)
    {
        point = new Point();
        var match = RegexCache.POINT_REGEX.Match(str);

        if (!match.Success)
            return false;

        if (!ushort.TryParse(match.Groups[1].Value, out var x))
            return false;

        if (!ushort.TryParse(match.Groups[2].Value, out var y))
            return false;

        point = (x, y);

        return true;
    }
}