using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ExitRequest" /> packet
/// </summary>
public sealed record ExitRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     Whether or not this is a request or not. The player has not yet completed logging out if it is a request.
    /// </summary>
    public required bool IsRequest { get; set; }
}