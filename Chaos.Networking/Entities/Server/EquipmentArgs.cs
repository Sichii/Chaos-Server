using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record EquipmentArgs : ISendArgs
{
    public ItemInfo Item { get; set; } = null!;
    public EquipmentSlot Slot { get; set; }
}