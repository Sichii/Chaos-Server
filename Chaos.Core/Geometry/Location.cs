using Chaos.Core.Definitions;
using Chaos.Core.Extensions;

namespace Chaos.Core.Geometry;

public readonly struct Location : IEquatable<Location>
{
    public string MapId { get; }

    public Point Point { get; }
    public static bool operator ==(Location left, Location right) => left.Equals(right);

    public static implicit operator Location(ValueTuple<string, int, int> tuple) =>
        new(tuple.Item1, ((ushort)tuple.Item2, (ushort)tuple.Item3));

    public static implicit operator Location(ValueTuple<string, Point> tuple) => new(tuple.Item1, tuple.Item2);
    public static bool operator !=(Location left, Location right) => !left.Equals(right);

    /// <summary>
    ///     Json & Master constructor for a structure representing a point in the world, which is a point paired with a map ID.
    /// </summary>
    private Location(string mapId, Point point)
    {
        MapId = mapId;
        Point = point;
    }

    /// <summary>
    ///     Gets this location's distance from another location.
    /// </summary>
    public int Distance(Location loc)
    {
        if (!loc.MapId.EqualsI(MapId))
            return int.MaxValue;

        return Distance(loc.MapId, loc.Point.X, loc.Point.Y);
    }

    /// <summary>
    ///     Gets this locations's distance from another location.
    /// </summary>
    public int Distance(string mapId, ushort x, ushort y)
    {
        if (!mapId.EqualsI(MapId))
            return int.MaxValue;

        return Math.Abs(x - Point.X) + Math.Abs(y - Point.Y);
    }

    public bool Equals(Location other) => MapId.EqualsI(other.MapId) && Point.Equals(other.Point);

    public override bool Equals(object? obj) => obj is Location other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(MapId), Point);

    public override string ToString() => $"{MapId}:{Point}";

    /// <summary>
    ///     Attempts to parse a location from a string.
    /// </summary>
    public static bool TryParse(string str, out Location? location)
    {
        location = null;
        var match = RegexCache.LOCATION_REGEX.Match(str);

        if (!match.Success)
            return false;

        if (!ushort.TryParse(match.Groups[2].Value, out var x))
            return false;

        if (!ushort.TryParse(match.Groups[3].Value, out var y))
            return false;

        location = (match.Groups[1].Value, x, y);

        return true;
    }
}