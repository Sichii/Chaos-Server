#region
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Synchronized;

/// <summary>
///     Wraps a <see cref="System.Collections.Generic.HashSet{T}" />, entering a lock for each of it's methods. Enumeration
///     will occur on a snapshot.
/// </summary>
/// <inheritdoc cref="System.Collections.Generic.HashSet{T}" />
public class SynchronizedHashSet<T> : ISet<T>, IReadOnlySet<T>
{
    private readonly HashSet<T> Set;
    private readonly Lock Sync;

    /// <inheritdoc cref="ICollection{T}.Count" />
    public int Count
    {
        get
        {
            using var @lock = Sync.EnterScope();
            var ret = Set.Count;

            return ret;
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <summary>
    ///     Creates a new <see cref="SynchronizedHashSet{T}" />.
    /// </summary>
    public SynchronizedHashSet(IEnumerable<T>? set = null, IEqualityComparer<T>? comparer = null)
    {
        Sync = new Lock();
        set ??= [];
        comparer ??= EqualityComparer<T>.Default;

        Set = new HashSet<T>(set, comparer);
    }

    /// <summary>
    ///     Creates a new <see cref="SynchronizedHashSet{T}" />.
    /// </summary>
    [JsonConstructor]
    public SynchronizedHashSet(IEnumerable<T> set)
    {
        Sync = new Lock();
        Set = new HashSet<T>(set);
    }

    void ICollection<T>.Add(T item) => Add(item);

    /// <inheritdoc />
    public bool Add(T item)
    {
        using var @lock = Sync.EnterScope();

        return Set.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        using var @lock = Sync.EnterScope();
        Set.Clear();
    }

    /// <inheritdoc cref="IReadOnlySet{T}.Contains" />
    public bool Contains(T item)
    {
        using var @lock = Sync.EnterScope();

        return Set.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.EnterScope();
        Set.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public void ExceptWith(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();
        Set.ExceptWith(other);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    public IEnumerator<T> GetEnumerator()
    {
        List<T> snapshot;

        using (Sync.EnterScope())
            snapshot = Set.ToList();

        foreach (var item in snapshot)
            yield return item;
    }

    /// <inheritdoc />
    public void IntersectWith(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();
        Set.IntersectWith(other);
    }

    /// <inheritdoc cref="ISet{T}.IsProperSubsetOf" />
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.IsProperSubsetOf(other);
    }

    /// <inheritdoc cref="ISet{T}.IsProperSupersetOf" />
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.IsProperSupersetOf(other);
    }

    /// <inheritdoc cref="ISet{T}.IsSubsetOf" />
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.IsSubsetOf(other);
    }

    /// <inheritdoc cref="ISet{T}.IsSupersetOf" />
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.IsSupersetOf(other);
    }

    /// <inheritdoc cref="ISet{T}.Overlaps" />
    public bool Overlaps(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.Overlaps(other);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        using var @lock = Sync.EnterScope();

        return Set.Remove(item);
    }

    /// <inheritdoc cref="ISet{T}.SetEquals" />
    public bool SetEquals(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();

        return Set.SetEquals(other);
    }

    /// <inheritdoc />
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();
        Set.SymmetricExceptWith(other);
    }

    /// <inheritdoc />
    public void UnionWith(IEnumerable<T> other)
    {
        using var @lock = Sync.EnterScope();
        Set.UnionWith(other);
    }

    /// <inheritdoc cref="HashSet{T}.TryGetValue" />
    public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue)
    {
        using var @lock = Sync.EnterScope();

        return Set.TryGetValue(equalValue, out actualValue);
    }
}