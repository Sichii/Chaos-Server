using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record ConnectionInfoSerializer : ServerPacketSerializer<ConnectionInfoArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ConnectionInfo;

    public override void Serialize(ref SpanWriter writer, ConnectionInfoArgs args)
    {
        writer.WriteByte(0);
        writer.WriteUInt32(args.TableCheckSum);
        writer.WriteByte(args.Seed);
        writer.WriteData8(args.Key);
    }
}