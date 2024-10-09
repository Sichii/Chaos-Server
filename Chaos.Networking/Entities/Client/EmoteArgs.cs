using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Emote" /> packet
/// </summary>
public sealed record EmoteArgs : IPacketSerializable
{
    /// <summary>
    ///     The body animation the client is requesting to be displayed.
    /// </summary>
    public required BodyAnimation BodyAnimation { get; set; }
}