using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record UnequipArgs : ISendArgs
{
    public EquipmentSlot EquipmentSlot { get; set; }
}