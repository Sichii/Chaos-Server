using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions;

public static class PointExtensions
{
    public static bool WithinRange(this IPoint point, IPoint other, int distance = 12)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return point.DistanceFrom(other) <= distance;
    }
}