using System.Runtime.CompilerServices;
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions;

public static class PointExtensions
{
    public static IEnumerable<IPoint> FilterByLineOfSight(this IEnumerable<IPoint> points, IPoint origin, MapInstance mapInstance)
    {
        ArgumentNullException.ThrowIfNull(points);

        ArgumentNullException.ThrowIfNull(origin);

        ArgumentNullException.ThrowIfNull(mapInstance);

        return points.Where(
            point => !mapInstance.IsWall(point)
                     && !origin.RayTraceTo(point)
                               .Any(pt => mapInstance.IsWall(pt)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this IPoint point, IPoint other, int distance = 15)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return point.ManhattanDistanceFrom(other) <= distance;
    }
}