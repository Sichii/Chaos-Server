using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions;

public static class LocationExtensions
{
    public static bool WithinRange(this ILocation location, ILocation other, int distance = 15)
    {
        var ret = location.WithinRange((IPoint)other, distance);

        if (!location.OnSameMapAs(other))
            return false;

        return ret;
    }
}