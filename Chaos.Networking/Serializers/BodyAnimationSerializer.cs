using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

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