using System.Runtime.CompilerServices;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions;

public static class LocationExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static bool WithinRange(this ILocation location, ILocation other, int distance = 13)
    {
        var ret = location.WithinRange((IPoint)other, distance);

        if (!location.OnSameMapAs(other))
            return false;

        return ret;
    }
}