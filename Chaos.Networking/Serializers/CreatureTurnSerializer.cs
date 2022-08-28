using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record CreatureTurnSerializer : ServerPacketSerializer<CreatureTurnArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.CreatureTurn;

    public override void Serialize(ref SpanWriter writer, CreatureTurnArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte((byte)args.Direction);
    }
}