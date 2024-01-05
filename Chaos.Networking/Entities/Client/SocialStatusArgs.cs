using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SocialStatus" />
///     packet
/// </summary>
public sealed record SocialStatusArgs : IPacketSerializable
{
    /// <summary>
    ///     The social status the client is trying to change their status to
    /// </summary>
    public required SocialStatus SocialStatus { get; set; }
}