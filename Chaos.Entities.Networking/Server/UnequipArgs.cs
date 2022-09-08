using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record UnequipArgs : ISendArgs
{
    public EquipmentSlot EquipmentSlot { get; set; }
}