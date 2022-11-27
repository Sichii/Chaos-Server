using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.JsonConverters;

namespace Chaos.Geometry;

[JsonConverter(typeof(LocationConverter))]
public readonly struct Location : ILocation, IEquatable<ILocation>
{
    public string Map { get; init; }
    public int X { get; init; }
    public int Y { get; init; }

    public static bool operator ==(Location left, ILocation right) => left.Equals(right);

    public static bool operator !=(Location left, ILocation right) => !left.Equals(right);

    public Location(string map, int x, int y)
    {
        X = x;
        Y = y;
        Map = map;
    }

    public void Deconstruct(out string map, out int x, out int y)
    {
        map = Map;
        x = X;
        y = Y;
    }

    public bool Equals(ILocation? other) => other is not null
                                            && (X == other.X)
                                            && (Y == other.Y)
                                            && (Map == other.Map);

    public override bool Equals(object? obj) => obj is ILocation other && Equals(other);

    public static Location From(ILocation location)
    {
        if (location is Location loc)
            return loc;

        return new Location(location.Map, location.X, location.Y);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y, Map);

    /// <inheritdoc />
    public override string ToString() => ILocation.ToString(this);

    public static bool TryParse(string str, out Location location)
    {
        location = new Location();
        var match = RegexCache.LOCATION_REGEX.Match(str);

        if (!match.Success)
            return false;

        if (!ushort.TryParse(match.Groups[2].Value, out var x))
            return false;

        if (!ushort.TryParse(match.Groups[3].Value, out var y))
            return false;

        location = new Location(match.Groups[1].Value, x, y);

        return true;
    }
}