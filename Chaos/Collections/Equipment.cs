using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;

namespace Chaos.Collections;

public sealed class Equipment : PanelBase<Item>, IEquipment
{
    public Equipment(IEnumerable<Item>? items = null)
        : base(
            PanelType.Equipment,
            19,
            new byte[] { 0 })
    {
        items ??= Array.Empty<Item>();

        foreach (var item in items)
            Objects[item.Slot] = item;
    }

    public Item? this[EquipmentSlot slot] => this[(byte)slot];

    public bool TryEquip(EquipmentType equipmentType, Item item, out Item? returnedItem)
    {
        returnedItem = null;

        if (equipmentType == EquipmentType.NotEquipment)
            throw new InvalidOperationException(
                $"Item {item.DisplayName} ({item.UniqueId}) has equipment type of {EquipmentType.NotEquipment}");

        using var @lock = Sync.Enter();

        var possibleSlots = equipmentType.ToEquipmentSlots();
        byte bSlot = 0;

        //check for empty slots
        foreach (var slot in possibleSlots)
        {
            bSlot = (byte)slot;

            if (TryAdd(bSlot, item))
                return true;
        }

        //no slots empty? try to replace
        TryGetRemove(bSlot, out returnedItem);
        TryAdd(bSlot, item);

        return true;
    }
}