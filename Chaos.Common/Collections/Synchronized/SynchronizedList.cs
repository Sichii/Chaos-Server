using System.Collections;
using Chaos.Common.Synchronization;

namespace Chaos.Common.Collections.Synchronized;

/// <summary>
///     Wraps a <see cref="System.Collections.Generic.List{T}" />, entering a lock for each of it's methods. Enumeration will occur on a
///     snapshot.
/// </summary>
/// <inheritdoc cref="System.Collections.Generic.List{T}" />
public class SynchronizedList<T> : IList<T>, IReadOnlyList<T>
{
    protected readonly List<T> List;
    protected readonly AutoReleasingMonitor Sync;

    public virtual T this[int index]
    {
        get
        {
            using var @lock = Sync.Enter();

            return List[index];
        }
        set
        {
            using var @lock = Sync.Enter();
            List[index] = value;
        }
    }

    public virtual int Count
    {
        get
        {
            using var @lock = Sync.Enter();

            return List.Count;
        }
    }

    public virtual bool IsReadOnly => false;

    public SynchronizedList(IEnumerable<T>? items = null)
    {
        Sync = new AutoReleasingMonitor();
        items ??= Enumerable.Empty<T>();
        List = new List<T>(items);
    }

    public virtual void Add(T item)
    {
        using var @lock = Sync.Enter();
        List.Add(item);
    }

    public virtual void Clear()
    {
        using var @lock = Sync.Enter();
        List.Clear();
    }

    public virtual bool Contains(T item)
    {
        using var @lock = Sync.Enter();

        return List.Contains(item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.Enter();
        List.CopyTo(array, arrayIndex);
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        using (Sync.Enter())
            snapshot = List.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual int IndexOf(T item)
    {
        using var @lock = Sync.Enter();

        return List.IndexOf(item);
    }

    public virtual void Insert(int index, T item)
    {
        using var @lock = Sync.Enter();
        List.Insert(index, item);
    }

    public virtual bool Remove(T item)
    {
        using var @lock = Sync.Enter();

        return List.Remove(item);
    }

    public virtual void RemoveAt(int index)
    {
        using var @lock = Sync.Enter();
        List.RemoveAt(index);
    }
}