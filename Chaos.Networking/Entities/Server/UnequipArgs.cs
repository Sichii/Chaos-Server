using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record UnequipArgs : ISendArgs
{
    public EquipmentSlot EquipmentSlot { get; set; }
}