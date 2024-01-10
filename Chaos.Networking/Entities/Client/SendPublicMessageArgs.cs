using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SendPublicMessage" /> packet
/// </summary>
public sealed record SendPublicMessageArgs : IPacketSerializable
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