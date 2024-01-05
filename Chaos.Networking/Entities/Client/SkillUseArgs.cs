using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UseSkill" />
///     packet
/// </summary>
public sealed record SkillUseArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the skill the client is trying to use
    /// </summary>
    public required byte SourceSlot { get; set; }
}