namespace Chaos.Collections.Specialized;

#region
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

/// <summary>
///     Represents a thread-safe fixed-size set of items. If the set is at capacity and a new item is added, the oldest
///     item is removed.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public sealed class FixedSet<T> : ICollection<T> where T: notnull
{
    private readonly LinkedList<T> Items;
    private readonly Dictionary<T, LinkedListNode<T>> Lookup;
    private readonly Lock Sync;

    /// <summary>
    ///     The number of items the set can hold
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    ///     The number of items in the collection.
    /// </summary>
    public int Count => Items.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FixedSet{T}" /> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">
    ///     The capacity of the set
    /// </param>
    /// <param name="items">
    ///     Items to populate the collection with
    /// </param>
    /// <param name="comparer">
    ///     The comparer to use for equality
    /// </param>
    public FixedSet(int capacity, IEnumerable<T>? items = null, IEqualityComparer<T>? comparer = null)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");

        items = items?.Distinct();
        items ??= [];

        Capacity = capacity;

        Items = [];
        Lookup = new Dictionary<T, LinkedListNode<T>>(comparer);
        Sync = new Lock();

        foreach (var item in items)
        {
            var node = Items.AddLast(item);
            Lookup.Add(item, node);
        }
    }

    /// <summary>
    ///     Adds an item to the collection
    /// </summary>
    /// <param name="item">
    ///     The item to add or update.
    /// </param>
    public void Add(T item)
    {
        using var @lock = Sync.EnterScope();

        // If the item already exists, remove its current node so we can re-insert at the end.
        if (Lookup.TryGetValue(item, out var existingNode))
        {
            Items.Remove(existingNode);

            // existingNode is now invalid after removal, 
            // we'll insert a new node at the end for clarity.
            Lookup.Remove(item);
        } else
        {
            // If the item is new and the capacity is full, remove oldest (front of the list).
            if (Items.Count == Capacity)
            {
                // Remove the first item from the list and from the dictionary
                var nodeToRemove = Items.First;

                if (nodeToRemove != null)
                {
                    Lookup.Remove(nodeToRemove.Value);
                    Items.RemoveFirst();
                }
            }
        }

        // Add the item to the end of the list and record it in the dictionary.
        var newNode = Items.AddLast(item);
        Lookup[item] = newNode;
    }

    /// <inheritdoc />
    public void Clear()
    {
        using var @lock = Sync.EnterScope();

        Items.Clear();
        Lookup.Clear();
    }

    /// <summary>
    ///     Returns true if the item exists in the collection, false otherwise.
    /// </summary>
    public bool Contains(T item)
    {
        using var @lock = Sync.EnterScope();

        return Lookup.ContainsKey(item);
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var @lock = Sync.EnterScope();

        Items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///     Returns an enumerator for the items in oldest-to-newest order.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        using var @lock = Sync.EnterScope();

        var snapshot = Items.ToList();

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Remove(T item)
    {
        using var @lock = Sync.EnterScope();

        if (!Lookup.Remove(item, out var node))
            return false;

        Items.Remove(node);

        return true;
    }
}