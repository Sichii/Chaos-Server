using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record AnimationSerializer : ServerPacketSerializer<AnimationArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Animation;

    public override void Serialize(ref SpanWriter writer, AnimationArgs args)
    {
        if (args.TargetPoint.HasValue)
        {
            //writer.WriteBytes(new byte[4]); //dunno
            writer.WriteUInt32(args.SourceId ?? 0);
            writer.WriteUInt16(args.TargetAnimation);
            writer.WriteUInt16(args.AnimationSpeed);
            writer.WritePoint16(args.TargetPoint.Value);
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