using Chaos.Containers.Abstractions;
using Chaos.Containers.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Services.Utility.Interfaces;

namespace Chaos.Containers;

public class Inventory : PanelBase<Item>, IInventory
{
    private readonly ICloningService<Item> ItemCloner;

    /// <summary>
    /// Used for character creation
    /// </summary>
    public Inventory()
        : this(null!) { }

    /// <summary>
    /// General use constructor
    /// </summary>
    public Inventory(ICloningService<Item> itemCloner)
        : base(
            PanelType.Inventory,
            60,
            new byte[] { 0 }) => ItemCloner = itemCloner;

    /// <summary>
    /// Used for converting from serial format
    /// </summary>
    public Inventory(
        ICloningService<Item> itemCloner,
        IEnumerable<SerializableItem> serializedItems,
        ISerialTransformService<Item, SerializableItem> itemTransformer
    )
        : this(itemCloner)
    {
        foreach (var serialized in serializedItems)
        {
            var item = itemTransformer.Transform(serialized);
            Objects[item.Slot] = item;
        }
    }

    public int CountOf(string name)
    {
        using var @lock = Sync.Enter();

        return this.Where(item => item.DisplayName.EqualsI(name)).Sum(item => item.Count);
    }

    public bool HasCount(string name, int quantity)
    {
        using var @lock = Sync.Enter();

        return CountOf(name) >= quantity;
    }

    public bool RemoveQuantity(string name, int quantity)
    {
        using var @lock = Sync.Enter();

        if (quantity <= 0)
            return false;

        var items = this
                    .Where(item => item.DisplayName.EqualsI(name))
                    .ToList();

        if (!items.Any())
            return false;

        var sum = items.Sum(item => item.Count);

        if (sum < quantity)
            return false;

        foreach (var item in items)
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

    public bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out ICollection<Item> items)
    {
        using var @lock = Sync.Enter();

        items = null;

        if (quantity <= 0)
            return false;

        var existingItems = this
                            .Where(item => item.DisplayName.EqualsI(name))
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

    public bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out Item item)
    {
        using var @lock = Sync.Enter();

        item = null;

        if (quantity <= 0)
            return false;

        if (!TryGetObject(slot, out var slotItem))
            return false;

        if (slotItem == null)
            return false;

        if (slotItem.Count < quantity)
            return false;

        if (slotItem.Count == quantity)
        {
            item = slotItem;

            return Remove(slot);
        }

        item = slotItem.Split(quantity, ItemCloner);
        Update(slot);

        return true;
    }

    public override bool TryAdd(byte slot, Item obj)
    {
        using var @lock = Sync.Enter();

        if (TryAddStackable(obj))
            return true;

        return base.TryAdd(slot, obj);
    }

    public bool TryAddDirect(byte slot, Item obj) => base.TryAdd(slot, obj);

    protected virtual bool TryAddStackable(Item obj)
    {
        if (!obj.Template.Stackable)
            return false;

        if (obj.Count == 0)
            obj.Count = 1;

        using var @lock = Sync.Enter();

        foreach (var item in this)
            if (item.Template.Name.Equals(obj.Template.Name, StringComparison.OrdinalIgnoreCase)
                && (item.Count < item.Template.MaxStacks))
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

    public override bool TryAddToNextSlot(Item obj)
    {
        using var @lock = Sync.Enter();

        if (TryAddStackable(obj))
            return true;

        return base.TryAddToNextSlot(obj);
    }
}