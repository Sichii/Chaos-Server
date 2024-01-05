using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GroupRequest" />
///     packet
/// </summary>
public sealed record GroupRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of the request
    /// </summary>
    public required GroupRequestType GroupRequestType { get; set; }

    /// <summary>
    ///     The name of the player the client is trying to send a group invite to
    /// </summary>
    public string TargetName { get; set; }
}