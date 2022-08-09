using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record EquipmentArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
    public EquipmentSlot Slot { get; set; }
}