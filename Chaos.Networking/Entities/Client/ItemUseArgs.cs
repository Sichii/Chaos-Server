using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UseItem" />
///     packet
/// </summary>
public sealed record ItemUseArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the item the client is trying to use
    /// </summary>
    public required byte SourceSlot { get; set; }
}