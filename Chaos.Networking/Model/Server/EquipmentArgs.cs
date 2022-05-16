using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record EquipmentArgs : ISendArgs
{
    public ItemArg Item { get; set; } = null!;
    public EquipmentSlot Slot { get; set; }
}