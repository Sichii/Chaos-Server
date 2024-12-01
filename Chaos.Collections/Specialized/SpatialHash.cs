#region
using System.Collections.Concurrent;
using Chaos.Collections.Synchronized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Collections.Specialized;

/// <summary>
///     A data structure used for fast point-based queries.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public sealed class SpatialHash<T> where T: IPoint
{
    private readonly IEqualityComparer<T>? Comparer;
    private readonly ConcurrentDictionary<Point, SynchronizedHashSet<T>> MapOfHashMaps = new();

    /// <summary>
    ///     Initializes a new instance of <see cref="SpatialHash{T}" />.
    /// </summary>
    /// <param name="comparer">
    ///     The comparer used to compare items
    /// </param>
    public SpatialHash(IEqualityComparer<T>? comparer = default) => Comparer = comparer;

    /// <summary>
    ///     Adds an item to the SpatialHash.
    /// </summary>
    /// <param name="item">
    ///     The item to be added
    /// </param>
    public void Add(T item)
    {
        var key = Point.From(item);

        if (!MapOfHashMaps.TryGetValue(key, out var hashMap))
        {
            hashMap = new SynchronizedHashSet<T>(comparer: Comparer);
            MapOfHashMaps.TryAdd(key, hashMap);
        }

        hashMap.Add(item);
    }

    /// <summary>
    ///     Clears the SpatialHash.
    /// </summary>
    public void Clear() => MapOfHashMaps.Clear();

    /// <summary>
    ///     Queries the SpatialHash for items at a given point.
    /// </summary>
    /// <param name="point">
    ///     The point to query at
    /// </param>
    /// <returns>
    ///     All items located at the given point
    /// </returns>
    public IEnumerable<T> Query(Point point)
    {
        if (MapOfHashMaps.TryGetValue(point, out var hashMap))
            return hashMap;

        return [];
    }

    /// <summary>
    ///     Removes an item from the SpatialHash.
    /// </summary>
    /// <param name="item">
    ///     The item to remove from the SpatialHash
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the item was found and removed from the SpatialHash, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Remove(T item)
    {
        var key = Point.From(item);

        if (!MapOfHashMaps.TryGetValue(key, out var hashMap))
            return false;

        if (hashMap.Remove(item))
        {
            if (hashMap.Count == 0)
                MapOfHashMaps.Remove(key, out _);

            return true;
        }

        return false;
    }
}