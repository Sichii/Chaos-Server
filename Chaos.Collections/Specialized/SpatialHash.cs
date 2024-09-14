using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;

namespace Chaos.Collections.Specialized;

/// <summary>
///     A data structure used for fast point-based queries.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public sealed class SpatialHash<T> where T: IPoint
{
    private readonly IEqualityComparer<T>? Comparer;
    private readonly Dictionary<IPoint, HashSet<T>> Hash = new(PointEqualityComparer.Instance);

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

        if (!Hash.TryGetValue(key, out var set))
        {
            set = new HashSet<T>(Comparer);
            Hash.Add(key, set);
        }

        set.Add(item);
    }

    /// <summary>
    ///     Clears the SpatialHash.
    /// </summary>
    public void Clear() => Hash.Clear();

    /// <summary>
    ///     Queries the SpatialHash for items at a given point.
    /// </summary>
    /// <param name="point">
    ///     The point to query at
    /// </param>
    /// <returns>
    ///     All items located at the given point
    /// </returns>
    public IEnumerable<T> Query(IPoint point)
    {
        if (Hash.TryGetValue(point, out var set))
            return set;

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

        if (!Hash.TryGetValue(key, out var set))
            return false;

        if (set.Remove(item))
        {
            if (set.Count == 0)
                Hash.Remove(key);

            return true;
        }

        return false;
    }
}