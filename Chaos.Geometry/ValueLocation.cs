#region
using System.Diagnostics.CodeAnalysis;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
#endregion

namespace Chaos.Geometry;

/// <summary>
///     Represents a location in a map
/// </summary>
public readonly ref struct ValueLocation : ILocation, IEquatable<ILocation>
{
    /// <inheritdoc />
    public string Map { get; init; }

    /// <inheritdoc />
    public int X { get; init; }

    /// <inheritdoc />
    public int Y { get; init; }

    /// <summary>
    ///     Creates a new location
    /// </summary>
    /// <param name="map">
    ///     The location's map
    /// </param>
    /// <param name="x">
    ///     The X coordinate
    /// </param>
    /// <param name="y">
    ///     The Y coordinate
    /// </param>
    public ValueLocation(string map, int x, int y)
    {
        X = x;
        Y = y;
        Map = map;
    }

    /// <summary>
    /// </summary>
    /// <param name="map">
    ///     The location's map
    /// </param>
    /// <param name="point">
    ///     The coordinate point
    /// </param>
    public ValueLocation(string map, Point point)
        : this(map, point.X, point.Y) { }

    /// <summary>
    ///     Creates a new location
    /// </summary>
    /// <param name="map">
    ///     The location's map
    /// </param>
    /// <param name="point">
    ///     The coordinate point
    /// </param>
    public ValueLocation(string map, IPoint point)
        : this(map, point.X, point.Y) { }

    /// <inheritdoc />
    public bool Equals(ILocation? other) => other is not null && (X == other.X) && (Y == other.Y) && (Map == other.Map);

    /// <summary>
    ///     Deconstructs a location
    /// </summary>
    /// <param name="map">
    /// </param>
    /// <param name="x">
    /// </param>
    /// <param name="y">
    /// </param>
    public void Deconstruct(out string map, out int x, out int y)
    {
        map = Map;
        x = X;
        y = Y;
    }

    /// <summary>
    ///     Implicitly converts a location to a ref struct location
    /// </summary>
    public static explicit operator ValueLocation(Location loc) => new(loc.Map, loc.X, loc.Y);

    /// <summary>
    ///     Creates a new <see cref="Chaos.Geometry.Location" /> from an existing
    ///     <see cref="Chaos.Geometry.Abstractions.ILocation" />
    /// </summary>
    /// <param name="location">
    ///     An implementation of ILocation
    /// </param>
    /// <returns>
    /// </returns>
    public static Location From(ILocation location)
    {
        if (location is Location loc)
            return loc;

        return new Location(location.Map, location.X, location.Y);
    }

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Map);

    /// <summary>
    ///     Compares two locations
    /// </summary>
    public static bool operator ==(ValueLocation left, ILocation right) => left.Equals(right);

    /// <summary>
    ///     Compares two locations
    /// </summary>
    public static bool operator !=(ValueLocation left, ILocation right) => !left.Equals(right);

    /// <inheritdoc />
    public override string ToString() => $"{Map}:({X}, {Y})";

    /// <inheritdoc />
    public override bool Equals(object? other) => other is ILocation loc && Equals(loc);

    /// <summary>
    ///     Tries to parse a location from a string
    /// </summary>
    /// <param name="str">
    /// </param>
    /// <param name="location">
    /// </param>
    /// <returns>
    /// </returns>
    public static bool TryParse(string str, [MaybeNullWhen(false)] out Location location)
    {
        location = null;
        var match = RegexCache.LocationRegex.Match(str);

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