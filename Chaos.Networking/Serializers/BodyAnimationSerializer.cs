using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record BodyAnimationSerializer : ServerPacketSerializer<BodyAnimationArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.BodyAnimation;

    public override void Serialize(ref SpanWriter writer, BodyAnimationArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte((byte)args.BodyAnimation);
        writer.WriteUInt16(args.Speed);
        writer.WriteByte(args.Sound);
    }
}