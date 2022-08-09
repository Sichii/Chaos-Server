using Chaos.Data;
using Chaos.Networking.Definitions;
using Chaos.Objects.Panel;

namespace Chaos.Containers.Interfaces;

public interface IEquipment : IPanel<Item>
{
    Item? this[EquipmentSlot slot] { get; }
    Attributes Modifiers { get; }

    bool TryEquip(Item item, out Item? returnedItem);
}