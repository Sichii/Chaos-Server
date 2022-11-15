using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Extensions.Geometry;

public static class LocationExtensions
{
    public static Location DirectionalOffset(this ILocation location, Direction direction, int distance = 1)
    {
        ArgumentNullException.ThrowIfNull(location);

        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up    => new Location(location.Map, location.X, location.Y - distance),
            Direction.Right => new Location(location.Map, location.X + 1, location.Y),
            Direction.Down  => new Location(location.Map, location.X, location.Y + 1),
            Direction.Left  => new Location(location.Map, location.X - 1, location.Y),
            _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static Direction DirectionalRelationTo(this ILocation location, ILocation other)
    {
        var ret = PointExtensions.DirectionalRelationTo(location, other);

        EnsureSameMap(location, other);

        return ret;
    }

    public static int DistanceFrom(this ILocation location, ILocation other)
    {
        var ret = PointExtensions.DistanceFrom(location, other);

        EnsureSameMap(location, other);

        return ret;
    }

    private static void EnsureSameMap(ILocation location1, ILocation location2)
    {
        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException(
                $"{ILocation.ToString(location1)} is not on the same map as {ILocation.ToString(location2)}");
    }

    public static Location OffsetTowards(this ILocation location, ILocation other)
    {
        ArgumentNullException.ThrowIfNull(location);

        ArgumentNullException.ThrowIfNull(other);

        EnsureSameMap(location, other);

        var direction = other.DirectionalRelationTo(location);

        return location.DirectionalOffset(direction);
    }

    public static bool OnSameMapAs(this ILocation location, ILocation other) =>
        location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
}