using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Unequip" /> packet
/// </summary>
public sealed record UnequipArgs : IPacketSerializable
{
    /// <summary>
    ///     The equipment slot of the item the client is trying to unequip
    /// </summary>
    public required EquipmentSlot EquipmentSlot { get; set; }
}