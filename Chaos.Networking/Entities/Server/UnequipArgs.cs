using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Unequip" />
///     packet
/// </summary>
public sealed record UnequipArgs : ISendArgs
{
    /// <summary>
    ///     The equipment slot to unequip
    /// </summary>
    public EquipmentSlot EquipmentSlot { get; set; }
}