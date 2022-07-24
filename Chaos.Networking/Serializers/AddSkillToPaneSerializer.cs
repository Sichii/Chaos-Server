using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record AddSkillToPaneSerializer : ServerPacketSerializer<AddSkillToPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.AddSkillToPane;

    public override void Serialize(ref SpanWriter writer, AddSkillToPaneArgs args)
    {
        writer.WriteByte(args.Skill.Slot);
        writer.WriteUInt16(args.Skill.Sprite);
        writer.WriteString8(args.Skill.Name);
    }
}