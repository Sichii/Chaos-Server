using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveSkillFromPaneSerializer : ServerPacketSerializer<RemoveSkillFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSkillFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSkillFromPaneArgs args) => writer.WriteByte(args.Slot);
}