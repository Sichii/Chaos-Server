using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Chant" /> packet
/// </summary>
public sealed record ChantArgs : IPacketSerializable
{
    /// <summary>
    ///     The chant to be displayed
    /// </summary>
    public required string ChantMessage { get; set; }
}