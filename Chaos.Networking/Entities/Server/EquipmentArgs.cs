using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.Equipment" />
///     packet
/// </summary>
public sealed record EquipmentArgs : ISendArgs
{
    /// <summary>
    ///     Details about the item being added to the equipment pane
    /// </summary>
    public ItemInfo Item { get; set; } = null!;
    /// <summary>
    ///     The equipment slot the item is being added to
    /// </summary>
    public EquipmentSlot Slot { get; set; }
}