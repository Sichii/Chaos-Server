using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UseSkill" /> packet
/// </summary>
/// <param name="SourceSlot">The slot of the skill the client is trying to use</param>
public sealed record SkillUseArgs(byte SourceSlot) : IReceiveArgs;