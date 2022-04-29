using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chaos.Containers.Abstractions;
using Chaos.Containers.Interfaces;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.PanelObjects;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Inventory : PanelBase<Item>, IPanelWithStacking<Item>
{
    public Inventory(ILogger logger)
        : base(PanelType.Inventory, 60, new byte[] { 0 }, logger) { }

    public bool RemoveQuantity(string name, int quantity)
    {
        lock (Sync)
        {
            var items = this
                .Where(item => item.DisplayName.EqualsI(name))
                .ToList();

            if (!items.Any())
                return false;

            var sum = items.Sum(item => item.Count);

            if (sum < quantity)
                return false;

            foreach (var item in items)
                if (item.Count <= quantity)
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
    }
    
    public bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out Item item)
    {
        lock (Sync)
        {
            item = null;
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

            item = slotItem.Split(quantity);
            Update(slot);

            return true;
        }
    }

    public override bool TryAdd(byte slot, Item obj)
    {
        lock (Sync)
        {
            if (TryAddStackable(obj))
                return true;

            return base.TryAdd(slot, obj);
        }
    }

    public bool TryAddDirect(byte slot, Item obj) => base.TryAdd(slot, obj);

    protected virtual bool TryAddStackable(Item obj)
    {
        if (!obj.Template.Stackable)
            return false;

        if (obj.Count == 0)
            obj.Count = 1;

        lock (Sync)
            foreach (var item in this)
                if (item.Template.Name.Equals(obj.Template.Name, StringComparison.OrdinalIgnoreCase))
                    if (item.Count < item.Template.MaxStacks)
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

                        //TODO: anything else to update?

                        if (obj.Count <= 0)
                            return true;
                    }

        return false;
    }

    public override bool TryAddToNextSlot(Item obj)
    {
        lock (Sync)
        {
            if (TryAddStackable(obj))
                return true;

            return base.TryAddToNextSlot(obj);
        }
    }
}