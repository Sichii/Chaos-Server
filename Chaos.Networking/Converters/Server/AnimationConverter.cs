using Chaos.Extensions.Networking;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="AnimationArgs" /> into a bufferee cref="AnimationArgs" />
/// </summary>
public sealed class AnimationConverter : PacketConverterBase<AnimationArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Animation;

    /// <inheritdoc />
    public override AnimationArgs Deserialize(ref SpanReader reader)
    {
        var targetId = reader.ReadUInt32();

        if (targetId == 0)
        {
            var targetAnimation = reader.ReadUInt16();
            var animationSpeed = reader.ReadUInt16();
            var targetPoints = reader.ReadPoint16();

            return new AnimationArgs
            {
                TargetAnimation = targetAnimation,
                AnimationSpeed = animationSpeed,
                TargetPoint = targetPoints
            };
        } else
        {
            var sourceId = reader.ReadUInt32();
            var targetAnimation = reader.ReadUInt16();
            var sourceAnimation = reader.ReadUInt16();
            var animationSpeed = reader.ReadUInt16();

            return new AnimationArgs
            {
                TargetId = targetId,
                SourceId = sourceId,
                TargetAnimation = targetAnimation,
                SourceAnimation = sourceAnimation,
                AnimationSpeed = animationSpeed
            };
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AnimationArgs args)
    {
        if (args.TargetPoint.HasValue)
        {
            writer.WriteUInt32(0);
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