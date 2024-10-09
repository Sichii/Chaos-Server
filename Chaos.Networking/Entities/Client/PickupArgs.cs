using Chaos.Geometry.Abstractions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Pickup" /> packet
/// </summary>
public sealed record PickupArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot the client is trying to pick up and item into
    /// </summary>
    public required byte DestinationSlot { get; set; }

    /// <summary>
    ///     The point from which the client is trying to pick up an item from
    /// </summary>
    public required IPoint SourcePoint { get; set; }
}