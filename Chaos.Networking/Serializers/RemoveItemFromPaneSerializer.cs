using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record RemoveItemFromPaneSerializer : ServerPacketSerializer<RemoveItemFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveItemFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveItemFromPaneArgs args) => writer.WriteByte(args.Slot);
}