using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveSpellFromPaneSerializer : ServerPacketSerializer<RemoveSpellFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSpellFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSpellFromPaneArgs args) => writer.WriteByte(args.Slot);
}