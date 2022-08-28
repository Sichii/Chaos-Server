using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record PublicMessageSerializer : ServerPacketSerializer<PublicMessageArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.PublicMessage;

    public override void Serialize(ref SpanWriter writer, PublicMessageArgs args)
    {
        writer.WriteByte((byte)args.PublicMessageType);
        writer.WriteUInt32(args.SourceId);
        writer.WriteString8(args.Message);
    }
}