using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SkillUseArgs" />
/// </summary>
public sealed record SkillUseDeserializer : ClientPacketDeserializer<SkillUseArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.UseSkill;

    /// <inheritdoc />
    public override SkillUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new SkillUseArgs(sourceSlot);
    }
}