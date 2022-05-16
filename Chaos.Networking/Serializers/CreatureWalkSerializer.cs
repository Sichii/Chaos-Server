using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record CreatureWalkSerializer : ServerPacketSerializer<CreatureWalkArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.CreatureWalk;

    public override void Serialize(ref SpanWriter writer, CreatureWalkArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WritePoint16(args.OldPoint);
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(0); //dunno
    }
}