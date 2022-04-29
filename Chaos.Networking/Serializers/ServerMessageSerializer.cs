using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record ServerMessageSerializer : ServerPacketSerializer<ServerMessageArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ServerMessage;

    public override void Serialize(ref SpanWriter writer, ServerMessageArgs args)
    {
        writer.WriteByte((byte)args.ServerMessageType);
        writer.WriteString16(args.Message);
    }
}