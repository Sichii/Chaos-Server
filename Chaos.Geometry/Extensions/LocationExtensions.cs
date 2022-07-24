using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;

namespace Chaos.Geometry.Extensions;

public static class LocationExtensions
{
    public static Location DirectionalOffset(this ILocation location, Direction direction, int distance = 1)
    {
        if (location == null)
            throw new ArgumentNullException(nameof(location));

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
    
    public static Location OffsetTowards(this ILocation location, ILocation other)
    {
        if (location == null)
            throw new ArgumentNullException(nameof(location));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        EnsureSameMap(location, other);

        var direction = other.DirectionalRelationTo(location);

        return location.DirectionalOffset(direction);
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

    public static bool OnSameMapAs(this ILocation location, ILocation other) =>
        location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
}