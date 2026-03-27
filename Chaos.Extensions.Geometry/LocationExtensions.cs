#region
using System.Runtime.CompilerServices;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ILocation" />.
/// </summary>
public static class LocationExtensions
{
    extension<T1>(T1 location1) where T1: ILocation, allows ref struct
    {
        /// <summary>
        ///     Ensures both locations are on the same map
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureSameMap<T2>(T2 location2) where T2: ILocation, allows ref struct
        {
            if (!location1.OnSameMapAs(location2))
                throw new InvalidOperationException($"{location1.ToString()} is not on the same map as {location2.ToString()}");
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
        public bool OnSameMapAs<T2>(T2 other) where T2: ILocation, allows ref struct
            => location1.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
    }
}