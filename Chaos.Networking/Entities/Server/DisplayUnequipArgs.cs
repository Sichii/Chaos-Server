using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayUnequip" /> packet
/// </summary>
public sealed record DisplayUnequipArgs : IPacketSerializable
{
    /// <summary>
    ///     The equipment slot to unequip
    /// </summary>
    public EquipmentSlot EquipmentSlot { get; set; }
}