using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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