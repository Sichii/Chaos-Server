using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.PublicMessage" /> packet
/// </summary>
public sealed record PublicMessageArgs : IPacketSerializable
{
    /// <summary>
    ///     The message the client is trying to send
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    ///     The type of the public message the client is trying to send
    /// </summary>
    public required PublicMessageType PublicMessageType { get; set; }
}