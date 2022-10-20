using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Networking.Extensions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record CreatureWalkSerializer : ServerPacketSerializer<CreatureWalkArgs>
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