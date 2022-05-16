using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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