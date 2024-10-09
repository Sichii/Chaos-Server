using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

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