using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record RemoveSkillFromPaneSerializer : ServerPacketSerializer<RemoveSkillFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSkillFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSkillFromPaneArgs args) => writer.WriteByte(args.Slot);
}