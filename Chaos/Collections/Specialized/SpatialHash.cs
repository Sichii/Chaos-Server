using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;

namespace Chaos.Collections.Specialized;

public sealed class SpatialHash<T> where T: IPoint
{
    private readonly IEqualityComparer<T>? Comparer;
    private readonly Dictionary<IPoint, HashSet<T>> Hash = new(PointEqualityComparer.Instance);

    public SpatialHash(IEqualityComparer<T>? comparer = default) => Comparer = comparer;

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

    public void Clear() => Hash.Clear();

    public IEnumerable<T> Query(IPoint point)
    {
        if (Hash.TryGetValue(point, out var set))
            return set;

        return [];
    }

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