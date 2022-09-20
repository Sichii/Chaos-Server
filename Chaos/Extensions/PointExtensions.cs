using System.Runtime.CompilerServices;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions;

public static class PointExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this IPoint point, IPoint other, int distance = 13)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return point.DistanceFrom(other) <= distance;
    }
}