using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.AddSkillToPane" /> packet
/// </summary>
public sealed record AddSkillToPaneArgs : IPacketSerializable
{
    /// <summary>
    ///     The info of the skill to add to the client's inventory
    /// </summary>
    public SkillInfo Skill { get; set; } = null!;
}