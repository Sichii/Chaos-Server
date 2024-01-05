using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Emote" /> packet
/// </summary>
public sealed record EmoteArgs : IPacketSerializable
{
    /// <summary>
    ///     The body animation the client is requesting to be displayed.
    /// </summary>
    public required BodyAnimation BodyAnimation { get; set; }
}