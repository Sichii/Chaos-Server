using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record EquipmentArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
    public EquipmentSlot Slot { get; set; }
}