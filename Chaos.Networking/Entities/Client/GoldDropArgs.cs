using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GoldDrop" />
///     packet
/// </summary>
public sealed record GoldDropArgs : IPacketSerializable
{
    /// <summary>
    ///     The amount of gold the client is trying to drop
    /// </summary>
    public required int Amount { get; set; }

    /// <summary>
    ///     The point the client is trying to drop the gold on
    /// </summary>
    public required IPoint DestinationPoint { get; set; }
}