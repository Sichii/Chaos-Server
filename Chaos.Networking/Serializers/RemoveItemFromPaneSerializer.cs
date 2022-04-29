using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record RemoveItemFromPaneSerializer : ServerPacketSerializer<RemoveItemFromPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveItemFromPane;
    public override void Serialize(ref SpanWriter writer, RemoveItemFromPaneArgs args) => writer.WriteByte(args.Slot);
}