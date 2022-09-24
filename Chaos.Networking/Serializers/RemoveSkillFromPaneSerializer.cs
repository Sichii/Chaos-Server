using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveSkillFromPaneSerializer : ServerPacketSerializer<RemoveSkillFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSkillFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSkillFromPaneArgs args) => writer.WriteByte(args.Slot);
}