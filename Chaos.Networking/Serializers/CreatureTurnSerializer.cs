using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

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