using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.JsonConverters;

namespace Chaos.Geometry;

[JsonConverter(typeof(PointConverter))]
public readonly struct Point : IPoint, IEquatable<IPoint>
{
    public int X { get; init; }
    public int Y { get; init; }

    public static bool operator ==(Point left, IPoint right) => left.Equals(right);

    public static implicit operator Point((byte X, byte Y) tuple) => new(tuple.X, tuple.Y);

    public static implicit operator Point((ushort X, ushort Y) tuple) => new(tuple.X, tuple.Y);

    public static bool operator !=(Point left, IPoint right) => !left.Equals(right);

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    private Point(IPoint point)
    {
        X = point.X;
        Y = point.Y;
    }

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public bool Equals(IPoint? other) => other is not null
                                         && (X == other.X)
                                         && (Y == other.Y);

    public override bool Equals(object? obj) => obj is IPoint other && Equals(other);

    public static Point From(IPoint point)
    {
        if (point is Point pt)
            return pt;

        return new Point(point);
    }

    public override int GetHashCode() => (X << 16) + Y;

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

        point = new Point(x, y);

        return true;
    }
}