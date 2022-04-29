using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Chaos.Core.Collections.Synchronized;

public class SynchronizedHashSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly HashSet<T> Set;
    private readonly object Sync = new();

    public int Count
    {
        get
        {
            lock (Sync)
                return Set.Count;
        }
    }

    public bool IsReadOnly => false;

    public SynchronizedHashSet(IEnumerable<T>? items = null, IEqualityComparer<T>? comparer = null)
    {
        items ??= Enumerable.Empty<T>();
        comparer ??= EqualityComparer<T>.Default;

        Set = new HashSet<T>(items, comparer);
    }

    void ICollection<T>.Add(T item) => Add(item);

    public bool Add(T item)
    {
        lock (Sync)
            return Set.Add(item);
    }

    public void Clear()
    {
        lock (Sync)
            Set.Clear();
    }

    public bool Contains(T item)
    {
        lock (Sync)
            return Set.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (Sync)
            Set.CopyTo(array, arrayIndex);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        lock (Sync)
            Set.ExceptWith(other);
    }

    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        lock (Sync)
            snapshot = Set.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void IntersectWith(IEnumerable<T> other)
    {
        lock (Sync)
            Set.IntersectWith(other);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.Overlaps(other);
    }

    public bool Remove(T item)
    {
        lock (Sync)
            return Set.Remove(item);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        lock (Sync)
            return Set.SetEquals(other);
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        lock (Sync)
            Set.SymmetricExceptWith(other);
    }

    public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
    {
        lock (Sync)
            return Set.TryGetValue(equalValue, out actualValue);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        lock (Sync)
            Set.UnionWith(other);
    }
}