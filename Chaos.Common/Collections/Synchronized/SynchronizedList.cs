#region
using System.Collections;
using System.Text.Json.Serialization;
#endregion

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
    ///     Object used to synchronize access to the <see cref="List" />
    /// </summary>
    protected readonly Lock Sync;

    /// <inheritdoc cref="IList{T}.this" />
    public virtual T this[int index]
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return List[index];
        }

        set
        {
            using var @lock = Sync.EnterScope();
            List[index] = value;
        }
    }

    /// <inheritdoc cref="ICollection{T}.Count" />
    public virtual int Count
    {
        get
        {
            using var @lock = Sync.EnterScope();

            return List.Count;
        }
    }

    /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
    public virtual bool IsReadOnly => false;

    /// <summary>
    ///     Creates a new <see cref="SynchronizedList{T}" />
    /// </summary>
    public SynchronizedList()
        : this([]) { }

    /// <summary>
    ///     Creates a new <see cref="SynchronizedList{T}" />
    /// </summary>
    /// <param name="list">
    ///     An optional sequence of initial items to add to the collection
    /// </param>
    [JsonConstructor]
    public SynchronizedList(IEnumerable<T> list)
    {
        Sync = new Lock();
        List = [..list];
    }

    /// <inheritdoc cref="ICollection{T}.Add" />
    public virtual void Add(T item)
    {
        using var @lock = Sync.EnterScope();
        List.Add(item);
    }

    /// <inheritdoc cref="ICollection{T}.Clear" />
    public virtual void Clear()
    {
        using var @lock = Sync.EnterScope();
        List.Clear();
    }

    /// <inheritdoc cref="ICollection{T}.Contains" />
    public virtual bool Contains(T item)
    {
        using var @lock = Sync.EnterScope();

        return List.Contains(item);
    }

    /// <inheritdoc cref="ICollection{T}.CopyTo" />
    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.EnterScope();
        List.CopyTo(array, arrayIndex);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public virtual IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        using (Sync.EnterScope())
            snapshot = List.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    /// <inheritdoc cref="IList{T}.IndexOf" />
    public virtual int IndexOf(T item)
    {
        using var @lock = Sync.EnterScope();

        return List.IndexOf(item);
    }

    /// <inheritdoc cref="IList{T}.Insert" />
    public virtual void Insert(int index, T item)
    {
        using var @lock = Sync.EnterScope();
        List.Insert(index, item);
    }

    /// <inheritdoc cref="ICollection{T}.Remove" />
    public virtual bool Remove(T item)
    {
        using var @lock = Sync.EnterScope();

        return List.Remove(item);
    }

    /// <inheritdoc cref="IList{T}.RemoveAt" />
    public virtual void RemoveAt(int index)
    {
        using var @lock = Sync.EnterScope();
        List.RemoveAt(index);
    }
}