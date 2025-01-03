#region
using Chaos.Models.Panel;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Collections;

public sealed class Bank : IEnumerable<Item>
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly Dictionary<string, Item> Items;
    private readonly Lock Sync;

    /// <summary>
    ///     The amount of gold in this bank
    /// </summary>
    public uint Gold { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bank" /> class.
    /// </summary>
    /// <param name="items">
    ///     Items to populate the bank with
    /// </param>
    public Bank(IEnumerable<Item>? items = null)
    {
        items ??= [];
        Sync = new Lock();
        Items = items.ToDictionary(item => item.DisplayName, StringComparer.OrdinalIgnoreCase);
        ItemCloner = null!;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Bank" /> class.
    /// </summary>
    /// <param name="items">
    ///     Items to populate the bank with
    /// </param>
    /// <param name="itemCloner">
    ///     A service used to clone items
    /// </param>
    public Bank(IEnumerable<Item>? items, ICloningService<Item> itemCloner)
        : this(items)
        => ItemCloner = itemCloner;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<Item> GetEnumerator()
    {
        List<Item> snapshot;

        using (Sync.EnterScope())
            snapshot = Items.Values.ToList();

        return snapshot.GetEnumerator();
    }

    /// <summary>
    ///     Adds gold to the bank
    /// </summary>
    /// <param name="amount">
    ///     The amount of gold to add
    /// </param>
    public void AddGold(uint amount)
    {
        using var @lock = Sync.EnterScope();

        Gold += amount;
    }

    /// <summary>
    ///     Determines whether the bank contains an item with the specified name
    /// </summary>
    /// <param name="itemName">
    ///     The name to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the bank contains an item with the specified name, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Contains(string itemName)
    {
        using var @lock = Sync.EnterScope();

        return Items.ContainsKey(itemName);
    }

    /// <summary>
    ///     Gets the amount of items with the specified name in the bank
    /// </summary>
    /// <param name="itemName">
    ///     The name of the item to look for
    /// </param>
    public int CountOf(string itemName)
    {
        using var @lock = Sync.EnterScope();

        if (Items.TryGetValue(itemName, out var item))
            return item.Count;

        return 0;
    }

    /// <summary>
    ///     Deposits an item into the bank
    /// </summary>
    /// <param name="item">
    ///     The item to deposit
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="item" /> count must be greater than 0
    /// </exception>
    public void Deposit(Item item)
    {
        using var @lock = Sync.EnterScope();

        if (item.Count < 1)
            throw new ArgumentOutOfRangeException($"{nameof(item)} count must be greater than 0");

        if (Items.TryGetValue(item.DisplayName, out var existingItem))
        {
            existingItem.Count += item.Count;
            item.Count = 0;
        } else
            Items.Add(item.DisplayName, item);
    }

    /// <summary>
    ///     Determines whether the bank has the specified amount of items with the specified name
    /// </summary>
    /// <param name="itemName">
    ///     The item name to check for
    /// </param>
    /// <param name="amount">
    ///     An amount of items to make this method return true
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the bank has the specified amount of items with the specified name, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasCount(string itemName, int amount) => CountOf(itemName) >= amount;

    /// <summary>
    ///     Removes gold from the bank
    /// </summary>
    /// <param name="amount">
    ///     The amount of gold to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the bank has the specified amount of gold, and it was removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool RemoveGold(uint amount)
    {
        using var @lock = Sync.EnterScope();

        if (Gold < amount)
            return false;

        Gold -= amount;

        return true;
    }

    private IEnumerable<Item> SplitItem(Item item, int amount)
    {
        if (item.Template.Stackable)
            item = item.Split(amount, ItemCloner);
        else
        {
            item.Count -= amount;
            item = ItemCloner.Clone(item);
            item.Count = amount;
        }

        return item.FixStacks(ItemCloner);
    }

    /// <summary>
    ///     Tries to withdraw an item from the bank
    /// </summary>
    /// <param name="itemName">
    ///     The name of the item to withdraw
    /// </param>
    /// <param name="amount">
    ///     The amount to withdraw
    /// </param>
    /// <param name="outItems">
    ///     The withdrawn items
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the item was found, there was enough of the item, and the item was withdrawn, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryWithdraw(string itemName, int amount, [MaybeNullWhen(false)] out IEnumerable<Item> outItems)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);

        using var @lock = Sync.EnterScope();

        outItems = null;

        if (!Items.TryGetValue(itemName, out var item) || (item.Count < amount))
            return false;

        if (item.Count == amount)
        {
            Items.Remove(itemName);
            outItems = item.FixStacks(ItemCloner);
        } else
            outItems = SplitItem(item, amount);

        return true;
    }
}