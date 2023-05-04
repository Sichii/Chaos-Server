using Chaos.Common.Definitions;
using Chaos.Models.Panel;

namespace Chaos.Collections.Abstractions;

public interface IEquipment : IPanel<Item>
{
    Item? this[EquipmentSlot slot] { get; }

    bool TryEquip(EquipmentType equipmentType, Item item, out Item? returnedItem);
}