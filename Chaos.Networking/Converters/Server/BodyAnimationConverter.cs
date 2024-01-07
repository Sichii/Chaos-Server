using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="BodyAnimationArgs" /> into a buffer
/// </summary>
public sealed class BodyAnimationConverter : PacketConverterBase<BodyAnimationArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.BodyAnimation;

    /// <inheritdoc />
    public override BodyAnimationArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();
        var bodyAnimation = reader.ReadByte();
        var animationSpeed = reader.ReadUInt16();
        var sound = reader.ReadByte();

        return new BodyAnimationArgs
        {
            SourceId = sourceId,
            BodyAnimation = (BodyAnimation)bodyAnimation,
            AnimationSpeed = animationSpeed,
            Sound = reader.ReadByte() == byte.MaxValue ? null : sound
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, BodyAnimationArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte((byte)args.BodyAnimation);
        writer.WriteUInt16(args.AnimationSpeed);
        writer.WriteByte(args.Sound ?? byte.MaxValue);
    }
}