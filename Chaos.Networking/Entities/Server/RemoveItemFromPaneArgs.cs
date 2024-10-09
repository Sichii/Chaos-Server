using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.RemoveItemFromPane" /> packet
/// </summary>
public sealed record RemoveItemFromPaneArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the item to be removed
    /// </summary>
    public byte Slot { get; set; }
}