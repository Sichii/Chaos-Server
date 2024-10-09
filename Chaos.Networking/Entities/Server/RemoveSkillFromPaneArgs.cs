using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.RemoveSkillFromPane" /> packet
/// </summary>
public sealed record RemoveSkillFromPaneArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the skill to be removed
    /// </summary>
    public byte Slot { get; set; }
}