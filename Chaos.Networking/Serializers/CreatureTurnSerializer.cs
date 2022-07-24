using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
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