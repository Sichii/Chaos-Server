using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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