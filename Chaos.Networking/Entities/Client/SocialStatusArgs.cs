using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SocialStatus" /> packet
/// </summary>
public sealed record SocialStatusArgs : IPacketSerializable
{
    /// <summary>
    ///     The social status the client is trying to change their status to
    /// </summary>
    public required SocialStatus SocialStatus { get; set; }
}