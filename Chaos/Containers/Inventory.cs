using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chaos.Containers.Abstractions;
using Chaos.Containers.Interfaces;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Objects.Panel;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Inventory : PanelBase<Item>, IInventory
{
    public Inventory()
        : base(
            PanelType.Inventory,
            60,
            new byte[] { 0 }) { }

    public int CountOf(string name)
    {
        lock (Sync)
            return this.Where(item => item.DisplayName.EqualsI(name)).Sum(item => item.Count);
    }

    public bool HasCount(string name, int quantity)
    {
        lock (Sync)
            return CountOf(name) >= quantity;
    }

    public bool RemoveQuantity(string name, int quantity)
    {
        lock (Sync)
        {
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
    }

    public bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out ICollection<Item> items)
    {
        lock (Sync)
        {
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
                    var split = item.Split(quantity);
                    BroadcastOnUpdated(item.Slot, item);
                    ret.Add(split);

                    break;
                }

            items = ret;

            return true;
        }
    }

    public bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out Item item)
    {
        lock (Sync)
        {
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