using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Collections;

public sealed class Inventory : PanelBase<Item>, IInventory
{
    private readonly ICloningService<Item> ItemCloner;

    /// <summary>
    ///     Used for character creation
    /// </summary>
    public Inventory(IEnumerable<Item>? items = null)
        : base(
            PanelType.Inventory,
            60,
            new byte[]
            {
                0
            })
    {
        ItemCloner = null!;
        items ??= Array.Empty<Item>();

        foreach (var item in items)
            Objects[item.Slot] = item;
    }

    /// <inheritdoc />
    public Inventory(ICloningService<Item> itemCloner, IEnumerable<Item>? items = null)
        : this(items)
        => ItemCloner = itemCloner;

    /// <inheritdoc />
    public override bool Contains(string name)
    {
        using var @lock = Sync.Enter();

        return Objects.Any(obj => obj is not null && obj.DisplayName.EqualsI(name));
    }

    /// <inheritdoc />
    public override bool Contains(Item obj) => Contains(obj.DisplayName);

    public int CountOf(string name)
    {
        using var @lock = Sync.Enter();

        return this.Where(item => item.DisplayName.EqualsI(name))
                   .Sum(item => item.Count);
    }

    /// <inheritdoc />
    public int CountOfByTemplateKey(string templateKey)
    {
        using var @lock = Sync.Enter();

        return this.Where(item => item.Template.TemplateKey.EqualsI(templateKey))
                   .Sum(item => item.Count);
    }

    public bool HasCount(string name, int quantity)
    {
        using var @lock = Sync.Enter();

        return CountOf(name) >= quantity;
    }

    /// <inheritdoc />
    public bool HasCountByTemplateKey(string templateKey, int quantity)
    {
        using var @lock = Sync.Enter();

        return CountOfByTemplateKey(templateKey) >= quantity;
    }

    public override Item? this[string name]
    {
        get
        {
            using var @lock = Sync.Enter();

            return this.FirstOrDefault(i => i.DisplayName.EqualsI(name));
        }
    }

    /// <inheritdoc />
    public override bool Remove(string name)
    {
        using var @lock = Sync.Enter();

        var obj = this.FirstOrDefault(obj => obj.DisplayName.EqualsI(name));

        if (obj == null)
            return false;

        return Remove(obj.Slot);
    }

    public bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out List<Item> items)
    {
        using var @lock = Sync.Enter();

        items = null;

        if (quantity <= 0)
            return false;

        var existingItems = this.Where(item => item.DisplayName.EqualsI(name))
                                .ToList();

        if (!existingItems.Any())
            return false;

        var sum = existingItems.Sum(item => item.Count);

        if (sum < quantity)
            return false;

        var ret = new List<Item>();

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
                ret.Add(item);
            } else
            {
                var split = item.Split(quantity, ItemCloner);
                BroadcastOnUpdated(item.Slot, item);
                ret.Add(split);

                break;
            }

        items = ret;

        return true;
    }

    public bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out List<Item> items)
    {
        using var @lock = Sync.Enter();

        items = null;

        if (quantity <= 0)
            return false;

        var existingItem = Objects[slot];

        if (existingItem == null)
            return false;

        if (!HasCount(existingItem.DisplayName, quantity))
            return false;

        var existingItems = Objects
                            .Where(item => (item != null) && item.DisplayName.EqualsI(existingItem.DisplayName) && (item.Slot != slot))
                            .Prepend(existingItem)
                            .ToList();

        var ret = new List<Item>();

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item!.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
                ret.Add(item);
            } else
            {
                var split = item.Split(quantity, ItemCloner);
                BroadcastOnUpdated(item.Slot, item);
                ret.Add(split);

                break;
            }

        items = ret;

        return true;
    }

    public bool RemoveQuantity(string name, int quantity)
    {
        using var @lock = Sync.Enter();

        if (quantity <= 0)
            return false;

        var existingItems = this.Where(item => item.DisplayName.EqualsI(name))
                                .ToList();

        if (!existingItems.Any())
            return false;

        var sum = existingItems.Sum(item => item.Count);

        if (sum < quantity)
            return false;

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
            } else
            {
                item.Count -= quantity;
                BroadcastOnUpdated(item.Slot, item);

                break;
            }

        return true;
    }

    public bool RemoveQuantity(byte slot, int quantity)
    {
        using var @lock = Sync.Enter();

        if (quantity <= 0)
            return false;

        var existingItem = Objects[slot];

        if (existingItem == null)
            return false;

        if (!HasCount(existingItem.DisplayName, quantity))
            return false;

        var existingItems = Objects
                            .Where(item => (item != null) && item.DisplayName.EqualsI(existingItem.DisplayName) && (item.Slot != slot))
                            .Prepend(existingItem)
                            .ToList();

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item!.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
            } else
            {
                item.Count -= quantity;
                BroadcastOnUpdated(item.Slot, item);

                break;
            }

        return true;
    }

    /// <inheritdoc />
    public bool RemoveQuantityByTemplateKey(string templateKey, int quantity, [MaybeNullWhen(false)] out List<Item> items)
    {
        using var @lock = Sync.Enter();

        items = null;

        if (quantity <= 0)
            return false;

        var existingItems = this.Where(item => item.Template.TemplateKey.EqualsI(templateKey))
                                .ToList();

        if (!existingItems.Any())
            return false;

        var sum = existingItems.Sum(item => item.Count);

        if (sum < quantity)
            return false;

        var ret = new List<Item>();

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
                ret.Add(item);
            } else
            {
                var split = item.Split(quantity, ItemCloner);
                BroadcastOnUpdated(item.Slot, item);
                ret.Add(split);

                break;
            }

        items = ret;

        return true;
    }

    /// <inheritdoc />
    public bool RemoveQuantityByTemplateKey(string templateKey, int quantity)
    {
        using var @lock = Sync.Enter();

        if (quantity <= 0)
            return false;

        var existingItems = this.Where(item => item.Template.TemplateKey.EqualsI(templateKey))
                                .ToList();

        if (!existingItems.Any())
            return false;

        var sum = existingItems.Sum(item => item.Count);

        if (sum < quantity)
            return false;

        foreach (var item in existingItems)
            if (quantity <= 0)
                break;
            else if (item.Count <= quantity)
            {
                Objects[item.Slot] = null;
                BroadcastOnRemoved(item.Slot, item);
                quantity -= item.Count;
            } else
            {
                item.Count -= quantity;
                BroadcastOnUpdated(item.Slot, item);

                break;
            }

        return true;
    }

    public override bool TryAdd(byte slot, Item obj)
    {
        using var @lock = Sync.Enter();

        var completed = false;

        if (!obj.Template.Stackable)
            completed |= base.TryAdd(slot, obj);

        if (completed || TryAddStackable(obj, slot))
            return true;

        return base.TryAddToNextSlot(obj);
    }

    public bool TryAddDirect(byte slot, Item obj) => base.TryAdd(slot, obj);

    public override bool TryAddToNextSlot(Item obj)
    {
        using var @lock = Sync.Enter();

        if (TryAddStackable(obj))
            return true;

        return base.TryAddToNextSlot(obj);
    }

    /// <inheritdoc />
    public override bool TryGetObject(string name, [MaybeNullWhen(false)] out Item obj)
    {
        using var @lock = Sync.Enter();

        obj = this.FirstOrDefault(obj => obj.DisplayName.EqualsI(name));

        return obj != null;
    }

    /// <inheritdoc />
    public override bool TryGetRemove(string name, [MaybeNullWhen(false)] out Item obj)
    {
        obj = default;

        using var @lock = Sync.Enter();

        obj = this.FirstOrDefault(obj => obj.DisplayName.EqualsI(name));

        if (obj == null)
            return false;

        Objects[obj.Slot] = default;
        BroadcastOnRemoved(obj.Slot, obj);

        return true;
    }

    /// <inheritdoc />
    public override bool TrySwap(byte slot1, byte slot2)
    {
        var item1 = Objects[slot1];
        var item2 = Objects[slot2];

        if ((item1 == null)
            || (item2 == null)
            || !item1.Template.Stackable
            || !item2.Template.Stackable
            || (item1.Count == item1.Template.MaxStacks)
            || (item2.Count == item2.Template.MaxStacks)
            || !item1.DisplayName.EqualsI(item2.DisplayName))
            return base.TrySwap(slot1, slot2);

        using var @lock = Sync.Enter();

        var missingStacks = item2.Template.MaxStacks - item2.Count;
        var stacksToGive = Math.Min(missingStacks, item1.Count);

        Update(slot2, i => i.Count += stacksToGive);

        if (stacksToGive == item1.Count)
            Remove(slot1);
        else
            Update(slot1, i => i.Count -= stacksToGive);

        return true;
    }

    private bool TryAddStackable(Item obj, byte? preferredSlot = null)
    {
        if (!obj.Template.Stackable)
            return false;

        if (preferredSlot.HasValue && !IsValidSlot(preferredSlot.Value))
            preferredSlot = null;

        if (obj.Count == 0)
            obj.Count = 1;

        using var @lock = Sync.Enter();

        var items = Objects.Where(i => i != null)
                           .ToList();

        //if there is a preferred slot, consider that slot first when adding
        if (preferredSlot.HasValue && (Objects[preferredSlot.Value] != null))
        {
            var preferredItem = Objects[preferredSlot.Value];

            items = items.Where(i => i!.Slot != preferredSlot.Value)
                         .Prepend(preferredItem)
                         .ToList();
        }

        foreach (var item in items)
            if (item!.DisplayName.Equals(obj.DisplayName, StringComparison.OrdinalIgnoreCase) && (item.Count < item.Template.MaxStacks))
            {
                var incomingStacks = obj.Count;
                var existingStacks = item.Count;
                var amountPossible = item.Template.MaxStacks - existingStacks;

                if (amountPossible == 0)
                    continue;

                var amountToAdd = Math.Clamp(obj.Count, 1, amountPossible);

                item.Count = (ushort)(existingStacks + amountToAdd);
                obj.Count = (ushort)Math.Clamp(incomingStacks - amountToAdd, 0, obj.Count);
                BroadcastOnUpdated(item.Slot, item);

                if (obj.Count <= 0)
                    return true;
            }

        return false;
    }
}