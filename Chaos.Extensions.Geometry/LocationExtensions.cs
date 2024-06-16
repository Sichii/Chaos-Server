using System.Runtime.CompilerServices;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ILocation" />.
/// </summary>
public static class LocationExtensions
{
    /// <summary>
    ///     Ensures both locations are on the same map
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public static void EnsureSameMap(ILocation location1, ILocation location2)
    {
        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException(
                $"{ILocation.ToString(location1)} is not on the same map as {ILocation.ToString(location2)}");
    }

    /// <summary>
    ///     Determines whether two <see cref="Chaos.Geometry.Abstractions.ILocation" /> are on the same map
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if both <see cref="Chaos.Geometry.Abstractions.ILocation" />s are on the same map, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OnSameMapAs(this ILocation location, ILocation other)
        => location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
}