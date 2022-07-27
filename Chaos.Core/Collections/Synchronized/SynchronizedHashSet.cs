using Chaos.Core.Synchronization;

namespace Chaos.Core.Collections.Synchronized;

public class SynchronizedHashSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly HashSet<T> Set;
    private readonly AutoReleasingMonitor Sync;

    public int Count
    {
        get
        {
            using var @lock = Sync.Enter();
            var ret = Set.Count;
            return ret;
        }
    }

    public bool IsReadOnly => false;

    public SynchronizedHashSet(IEnumerable<T>? items = null, IEqualityComparer<T>? comparer = null)
    {
        Sync = new AutoReleasingMonitor();
        items ??= Enumerable.Empty<T>();
        comparer ??= EqualityComparer<T>.Default;

        Set = new HashSet<T>(items, comparer);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public bool Add(T item)
    {
        using var @lock = Sync.Enter();

        return Set.Add(item);
    }

    public void Clear()
    {
        using var @lock = Sync.Enter();
        Set.Clear();
    }

    public bool Contains(T item)
    {
        using var @lock = Sync.Enter();

        return Set.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.Enter();
        Set.CopyTo(array, arrayIndex);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();
        Set.ExceptWith(other);
    }

    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        using (Sync.Enter())
            snapshot = Set.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void IntersectWith(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();
        Set.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.Overlaps(other);
    }

    public bool Remove(T item)
    {
        using var @lock = Sync.Enter();

        return Set.Remove(item);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();

        return Set.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();
        Set.SymmetricExceptWith(other);
    }

    public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
    {
        using var @lock = Sync.Enter();

        return Set.TryGetValue(equalValue, out actualValue);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        using var @lock = Sync.Enter();
        Set.UnionWith(other);
    }
}