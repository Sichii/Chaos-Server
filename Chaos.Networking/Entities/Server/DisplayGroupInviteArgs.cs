using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayGroupInvite" /> packet
/// </summary>
public sealed record DisplayGroupInviteArgs : IPacketSerializable
{
    /// <summary>
    ///     The group box information, if present
    /// </summary>
    public DisplayGroupBoxInfo? GroupBoxInfo { get; set; }

    /// <summary>
    ///     The type of the request
    /// </summary>
    public ServerGroupSwitch ServerGroupSwitch { get; set; }

    /// <summary>
    ///     The name of the source of the request
    /// </summary>
    public string SourceName { get; set; } = null!;
}