using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

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