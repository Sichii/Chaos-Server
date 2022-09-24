using Chaos.Common.Definitions;
using Chaos.Objects.Panel;

namespace Chaos.Containers.Abstractions;

public interface IEquipment : IPanel<Item>
{
    Item? this[EquipmentSlot slot] { get; }

    bool TryEquip(EquipmentType equipmentType, Item item, out Item? returnedItem);
}