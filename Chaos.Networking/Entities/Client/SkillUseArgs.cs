using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SkillUse" /> packet
/// </summary>
public sealed record SkillUseArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the skill the client is trying to use
    /// </summary>
    public required byte SourceSlot { get; set; }
}