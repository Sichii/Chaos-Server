using Chaos.Containers.Abstractions;
using Chaos.Containers.Interfaces;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Objects.Panel;
using Microsoft.Extensions.Logging;

namespace Chaos.Containers;

public class Equipment : PanelBase<Item>, IEquipment
{
    public Attributes Modifiers { get; }
    public Item? this[EquipmentSlot slot] => this[(byte)slot];

    public Equipment()
        : base(
            PanelType.Equipment,
            19,
            new byte[] { 0 }) => Modifiers = new Attributes();

    public bool TryEquip(Item item, out Item? returnedItem)
    {
        returnedItem = null;

        if (item.Template.EquipmentType is null or EquipmentType.NotEquipment)
            return false;

        lock (Sync)
        {
            var possibleSlots = item.Template.EquipmentType.Value.ToEquipmentSlots();
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
}