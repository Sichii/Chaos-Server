using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record UnequipArgs : ISendArgs
{
    public EquipmentSlot EquipmentSlot { get; set; }
}