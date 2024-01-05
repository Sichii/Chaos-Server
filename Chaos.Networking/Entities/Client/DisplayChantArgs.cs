using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Chant" /> packet
/// </summary>
public sealed record DisplayChantArgs : IPacketSerializable
{
    /// <summary>
    ///     The chant to be displayed
    /// </summary>
    public required string ChantMessage { get; set; }
}