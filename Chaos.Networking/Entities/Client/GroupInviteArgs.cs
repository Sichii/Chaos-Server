using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.GroupInvite" /> packet
/// </summary>
public sealed record GroupInviteArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of the request
    /// </summary>
    public required GroupRequestType GroupRequestType { get; set; }

    /// <summary>
    ///     The name of the player the client is trying to send a group invite to
    /// </summary>
    public string TargetName { get; set; } = null!;
}