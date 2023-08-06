using Chaos.Extensions.Networking;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="AnimationArgs" /> into a buffer
/// </summary>
public sealed record AnimationSerializer : ServerPacketSerializer<AnimationArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Animation;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AnimationArgs args)
    {
        if (args.TargetPoint.HasValue)
        {
            writer.WriteUInt32(0); //TODO: explore what this does, setting to sourceId resulted in a random animation on the source
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