using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.GroupInvite" /> packet
/// </summary>
public sealed record GroupInviteArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of the request
    /// </summary>
    public required ClientGroupSwitch ClientGroupSwitch { get; set; }

    /// <summary>
    ///     The group box information, if present
    /// </summary>
    public CreateGroupBoxInfo? GroupBoxInfo { get; set; } = null!;

    /// <summary>
    ///     The name of the player the client is trying to send a group invite to. Will be the name of the current player if
    ///     creating a group box.
    /// </summary>
    public string TargetName { get; set; } = null!;
}