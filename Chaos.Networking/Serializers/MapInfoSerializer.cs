using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record MapInfoSerializer : ServerPacketSerializer<MapInfoArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.MapInfo;

    public override void Serialize(ref SpanWriter writer, MapInfoArgs args)
    {
        writer.WriteInt16(args.MapId);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteByte((byte)args.Flags);
        writer.WriteBytes(new byte[2]);
        writer.WriteUInt16(args.CheckSum);
        writer.WriteString8(args.Name);
    }
}