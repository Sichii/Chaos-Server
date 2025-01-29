#region
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ILocation" />.
/// </summary>
public static class LocationExtensions
{
    #region Location EnsureSameMap
    /// <inheritdoc cref="EnsureSameMap(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureSameMap(ValueLocation location1, ValueLocation location2)
    {
        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException($"{location1.ToString()} is not on the same map as {location2.ToString()}");
    }

    /// <inheritdoc cref="EnsureSameMap(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureSameMap(ValueLocation location1, ILocation location2)
    {
        ArgumentNullException.ThrowIfNull(location2);

        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException($"{location1.ToString()} is not on the same map as {ILocation.ToString(location2)}");
    }

    /// <inheritdoc cref="EnsureSameMap(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureSameMap(ILocation location1, ValueLocation location2)
    {
        ArgumentNullException.ThrowIfNull(location1);

        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException($"{ILocation.ToString(location1)} is not on the same map as {location2.ToString()}");
    }

    /// <summary>
    ///     Ensures both locations are on the same map
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureSameMap(ILocation location1, ILocation location2)
    {
        ArgumentNullException.ThrowIfNull(location1);

        ArgumentNullException.ThrowIfNull(location2);

        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException(
                $"{ILocation.ToString(location1)} is not on the same map as {ILocation.ToString(location2)}");
    }
    #endregion

    #region Location OnSameMapAs
    /// <inheritdoc cref="OnSameMapAs(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OnSameMapAs(this ValueLocation location, ValueLocation other)
        => location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc cref="OnSameMapAs(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OnSameMapAs(this ValueLocation location, ILocation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc cref="OnSameMapAs(ILocation, ILocation)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OnSameMapAs(this ILocation location, ValueLocation other)
    {
        ArgumentNullException.ThrowIfNull(location);

        return location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
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
    {
        ArgumentNullException.ThrowIfNull(location);

        ArgumentNullException.ThrowIfNull(other);

        return location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
    }
    #endregion
}