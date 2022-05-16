using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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