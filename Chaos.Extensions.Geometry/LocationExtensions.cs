using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ILocation" />.
/// </summary>
public static class LocationExtensions
{
    /// <summary>
    ///     Offsets an <see cref="Chaos.Geometry.Abstractions.ILocation" /> in the specified
    ///     <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> by the specified <paramref name="distance" />
    /// </summary>
    /// <param name="location">The location to offset</param>
    /// <param name="direction">The direction to offset to</param>
    /// <param name="distance">The distance to offset by</param>
    /// <returns>
    ///     A new <see cref="Chaos.Geometry.Location" /> offset <paramref name="distance" /> number of tiles in <paramref name="direction" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Location DirectionalOffset(this ILocation location, Direction direction, int distance = 1)
    {
        ArgumentNullException.ThrowIfNull(location);

        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up    => new Location(location.Map, location.X, location.Y - distance),
            Direction.Right => new Location(location.Map, location.X + distance, location.Y),
            Direction.Down  => new Location(location.Map, location.X, location.Y + distance),
            Direction.Left  => new Location(location.Map, location.X - distance, location.Y),
            _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    /// <summary>
    ///     Determines the directional relationship between this <see cref="Chaos.Geometry.Abstractions.ILocation" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.ILocation" />
    /// </summary>
    /// <param name="location">The <see cref="Chaos.Geometry.Abstractions.ILocation" /> whose relation to another to find</param>
    /// <param name="other">The <see cref="Chaos.Geometry.Abstractions.ILocation" /> to find the relation to</param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> <paramref name="other" /> would need to face to be facing
    ///     <paramref name="location" />
    /// </returns>
    public static Direction DirectionalRelationTo(this ILocation location, ILocation other)
    {
        var ret = PointExtensions.DirectionalRelationTo(location, other);

        EnsureSameMap(location, other);

        return ret;
    }

    /// <summary>
    ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.ILocation" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.ILocation" />
    /// </summary>
    /// <param name="location"></param>
    /// <param name="other">The <see cref="Chaos.Geometry.Abstractions.ILocation" /> to check distance against</param>
    /// <returns>The distance between the two given locations without moving diagonally</returns>
    public static int DistanceFrom(this ILocation location, ILocation other)
    {
        var ret = PointExtensions.DistanceFrom(location, other);

        EnsureSameMap(location, other);

        return ret;
    }

    /// <summary>
    ///     Ensures both locations are on the same map
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private static void EnsureSameMap(ILocation location1, ILocation location2)
    {
        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException(
                $"{ILocation.ToString(location1)} is not on the same map as {ILocation.ToString(location2)}");
    }

    /// <summary>
    ///     Offsets one <see cref="Chaos.Geometry.Abstractions.ILocation" /> towards another <see cref="Chaos.Geometry.Abstractions.ILocation" />
    /// </summary>
    /// <param name="location"></param>
    /// <param name="other">The location to offset towards</param>
    /// <returns>A new <see cref="Chaos.Geometry.Location" /> that has been offset in the direction of <paramref name="other" /></returns>
    public static Location OffsetTowards(this ILocation location, ILocation other)
    {
        ArgumentNullException.ThrowIfNull(location);

        ArgumentNullException.ThrowIfNull(other);

        EnsureSameMap(location, other);

        var direction = other.DirectionalRelationTo(location);

        return location.DirectionalOffset(direction);
    }

    /// <summary>
    ///     Determines whether two <see cref="Chaos.Geometry.Abstractions.ILocation" /> are on the same map
    /// </summary>
    /// <returns><c>true</c> if both <see cref="Chaos.Geometry.Abstractions.ILocation" />s are on the same map, otherwise <c>false</c></returns>
    public static bool OnSameMapAs(this ILocation location, ILocation other) =>
        location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
}