using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.GroupInvite" /> packet
/// </summary>
public sealed record GroupRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of the request
    /// </summary>
    public GroupRequestType GroupRequestType { get; set; }

    /// <summary>
    ///     The name of the source of the request
    /// </summary>
    public string SourceName { get; set; } = null!;
}