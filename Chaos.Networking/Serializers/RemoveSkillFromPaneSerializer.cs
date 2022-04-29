using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveSkillFromPaneSerializer : ServerPacketSerializer<RemoveSkillFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSkillFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSkillFromPaneArgs args) => writer.WriteByte(args.Slot);
}