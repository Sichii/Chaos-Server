using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record SkillUseDeserializer : ClientPacketDeserializer<SkillUseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.UseSkill;

    public override SkillUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new SkillUseArgs(sourceSlot);
    }
}