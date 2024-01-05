using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ItemDrop" />
///     packet
/// </summary>
public sealed record ItemDropArgs : IPacketSerializable
{
    /// <summary>
    ///     The amount of the item the client is trying to drop
    /// </summary>
    public required int Count { get; set; }

    /// <summary>
    ///     The point the client is trying to drop the item on
    /// </summary>
    public required IPoint DestinationPoint { get; set; }

    /// <summary>
    ///     The slot of the item the client is trying to drop
    /// </summary>
    public required byte SourceSlot { get; set; }
}