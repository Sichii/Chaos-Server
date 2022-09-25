using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveSpellFromPaneSerializer : ServerPacketSerializer<RemoveSpellFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveSpellFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveSpellFromPaneArgs args) => writer.WriteByte(args.Slot);
}