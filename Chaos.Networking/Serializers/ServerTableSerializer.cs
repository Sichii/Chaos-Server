using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record ServerTableSerializer : ServerPacketSerializer<ServerTableArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.ServerTable;

    public override void Serialize(ref SpanWriter writer, ServerTableArgs args)
    {
        writer.WriteData16(args.ServerTable);
        writer.WriteByte(2);
    }
}