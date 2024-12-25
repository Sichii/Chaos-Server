#region
using System.Runtime.CompilerServices;
using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
#endregion

namespace Chaos.Extensions;

public static class PointExtensions
{
    [OverloadResolutionPriority(1)]
    public static IEnumerable<Point> FilterByLineOfSight(
        this IEnumerable<Point> points,
        Point origin,
        MapInstance mapInstance,
        bool invertLos = false)
    {
        ArgumentNullException.ThrowIfNull(points);

        ArgumentNullException.ThrowIfNull(mapInstance);

        if (!invertLos)
            return points.Where(
                point => !origin.RayTraceTo(point)
                                .Any(mapInstance.IsWall));

        points = points.ToList();

        var occludedPoints = points.Where(mapInstance.IsWall)
                                   .SelectMany(point => point.RayTraceTo(origin))
                                   .ToHashSet();

        return points.Except(occludedPoints);
    }

    public static IEnumerable<T> FilterByLineOfSight<T>(
        this IEnumerable<T> points,
        IPoint origin,
        MapInstance mapInstance,
        bool invertLos = false) where T: IPoint
    {
        ArgumentNullException.ThrowIfNull(points);

        ArgumentNullException.ThrowIfNull(origin);

        ArgumentNullException.ThrowIfNull(mapInstance);

        var pointSet = points.OfType<IPoint>()
                             .ToHashSet(PointEqualityComparer.Instance);

        foreach (var point in FilterByLineOfSight(
                     pointSet.Select(Point.From),
                     Point.From(origin),
                     mapInstance,
                     invertLos))
            if (pointSet.TryGetValue(point, out var setPoint))
                yield return (T)setPoint;
    }

    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this Point point, Point other, int distance = 15) => point.ManhattanDistanceFrom(other) <= distance;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this IPoint point, IPoint other, int distance = 15)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return WithinRange(Point.From(point), Point.From(other), distance);
    }
}