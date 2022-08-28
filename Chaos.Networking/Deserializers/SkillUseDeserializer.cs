using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record SkillUseDeserializer : ClientPacketDeserializer<SkillUseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SkillUse;

    public override SkillUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new SkillUseArgs(sourceSlot);
    }
}