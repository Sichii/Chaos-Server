using System.Collections;
using Chaos.Common.Synchronization;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Synchronized;

/// <summary>
///     Wraps a <see cref="System.Collections.Generic.List{T}" />, entering a lock for each of it's methods. Enumeration
///     will occur on a snapshot.
/// </summary>
/// <inheritdoc cref="System.Collections.Generic.List{T}" />
public class SynchronizedList<T> : IList<T>, IReadOnlyList<T>
{
    /// <summary>
    ///     The underlying <see cref="System.Collections.Generic.List{T}" />
    /// </summary>
    protected readonly List<T> List;

    /// <summary>
    ///     The <see cref="AutoReleasingMonitor" /> used to synchronize access to the <see cref="List" />
    /// </summary>
    protected readonly AutoReleasingMonitor Sync;

    /// <inheritdoc cref="IList{T}.this" />
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

    /// <inheritdoc cref="ICollection{T}.Count" />
    public virtual int Count
    {
        get
        {
            using var @lock = Sync.Enter();

            return List.Count;
        }
    }

    /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
    public virtual bool IsReadOnly => false;

    /// <summary>
    ///     Creates a new <see cref="SynchronizedList{T}" />
    /// </summary>
    /// <param name="items">
    ///     An optional sequence of initial items to add to the collection
    /// </param>
    public SynchronizedList(IEnumerable<T>? items = null)
    {
        Sync = new AutoReleasingMonitor();
        items ??= Enumerable.Empty<T>();
        List = new List<T>(items);
    }

    /// <inheritdoc cref="ICollection{T}.Add" />
    public virtual void Add(T item)
    {
        using var @lock = Sync.Enter();
        List.Add(item);
    }

    /// <inheritdoc cref="ICollection{T}.Clear" />
    public virtual void Clear()
    {
        using var @lock = Sync.Enter();
        List.Clear();
    }

    /// <inheritdoc cref="ICollection{T}.Contains" />
    public virtual bool Contains(T item)
    {
        using var @lock = Sync.Enter();

        return List.Contains(item);
    }

    /// <inheritdoc cref="ICollection{T}.CopyTo" />
    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.Enter();
        List.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public virtual IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        using (Sync.Enter())
            snapshot = List.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    /// <inheritdoc cref="IList{T}.IndexOf" />
    public virtual int IndexOf(T item)
    {
        using var @lock = Sync.Enter();

        return List.IndexOf(item);
    }

    /// <inheritdoc cref="IList{T}.Insert" />
    public virtual void Insert(int index, T item)
    {
        using var @lock = Sync.Enter();
        List.Insert(index, item);
    }

    /// <inheritdoc cref="ICollection{T}.Remove" />
    public virtual bool Remove(T item)
    {
        using var @lock = Sync.Enter();

        return List.Remove(item);
    }

    /// <inheritdoc cref="IList{T}.RemoveAt" />
    public virtual void RemoveAt(int index)
    {
        using var @lock = Sync.Enter();
        List.RemoveAt(index);
    }
}