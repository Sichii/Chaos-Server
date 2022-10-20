using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record UnequipArgs : ISendArgs
{
    public EquipmentSlot EquipmentSlot { get; set; }
}