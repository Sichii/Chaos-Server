using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Unequip" />
///     packet
/// </summary>
public sealed record UnequipRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     The equipment slot of the item the client is trying to unequip
    /// </summary>
    public required EquipmentSlot EquipmentSlot { get; set; }
}