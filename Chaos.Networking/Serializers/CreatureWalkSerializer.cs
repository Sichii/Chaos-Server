using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;
using Chaos.Networking.Extensions;

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