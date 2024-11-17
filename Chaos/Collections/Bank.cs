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
    public uint Gold { get; private set; }

    public Bank(IEnumerable<Item>? items = null)
    {
        items ??= [];
        Sync = new Lock();
        Items = items.ToDictionary(item => item.DisplayName, StringComparer.OrdinalIgnoreCase);
        ItemCloner = null!;
    }

    public Bank(IEnumerable<Item>? items, ICloningService<Item> itemCloner)
        : this(items)
        => ItemCloner = itemCloner;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<Item> GetEnumerator()
    {
        List<Item> snapshot;

        using (Sync.EnterScope())
            snapshot = Items.Values.ToList();

        return snapshot.GetEnumerator();
    }

    public void AddGold(uint amount)
    {
        using var @lock = Sync.EnterScope();

        Gold += amount;
    }

    public bool Contains(string itemName)
    {
        using var @lock = Sync.EnterScope();

        return Items.ContainsKey(itemName);
    }

    public int CountOf(string itemName)
    {
        using var @lock = Sync.EnterScope();

        if (Items.TryGetValue(itemName, out var item))
            return item.Count;

        return 0;
    }

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

    public bool HasCount(string itemName, int amount) => CountOf(itemName) >= amount;

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

    public bool TryWithdraw(string itemName, int amount, [MaybeNullWhen(false)] out IEnumerable<Item> outItems)
    {
        if (amount < 1)
            throw new ArgumentOutOfRangeException($"{nameof(amount)} must be greater than 0");

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