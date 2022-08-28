using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Networking.Extensions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record AnimationSerializer : ServerPacketSerializer<AnimationArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Animation;

    public override void Serialize(ref SpanWriter writer, AnimationArgs args)
    {
        if (args.TargetPoint != null)
        {
            //writer.WriteBytes(new byte[4]); //dunno
            writer.WriteUInt32(args.SourceId ?? 0);
            writer.WriteUInt16(args.TargetAnimation);
            writer.WriteUInt16(args.AnimationSpeed);
            writer.WritePoint16(args.TargetPoint);
        } else
        {
            writer.WriteUInt32(args.TargetId ?? 0);
            writer.WriteUInt32(args.SourceId ?? 0);
            writer.WriteUInt16(args.TargetAnimation);
            writer.WriteUInt16(args.SourceAnimation);
            writer.WriteUInt16(args.AnimationSpeed);
        }
    }
}