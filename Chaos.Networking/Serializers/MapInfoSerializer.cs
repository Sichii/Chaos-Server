using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record MapInfoSerializer : ServerPacketSerializer<MapInfoArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.MapInfo;

    public override void Serialize(ref SpanWriter writer, MapInfoArgs args)
    {
        writer.WriteInt16(args.MapId);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteByte(args.Flags);
        writer.WriteBytes(new byte[2]);
        writer.WriteUInt16(args.CheckSum);
        writer.WriteString8(args.Name);
    }
}