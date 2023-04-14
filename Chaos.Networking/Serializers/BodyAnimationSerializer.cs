using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="BodyAnimationArgs" /> into a buffer
/// </summary>
public sealed record BodyAnimationSerializer : ServerPacketSerializer<BodyAnimationArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.BodyAnimation;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, BodyAnimationArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte((byte)args.BodyAnimation);
        writer.WriteUInt16(args.AnimationSpeed);
        writer.WriteByte(args.Sound ?? byte.MaxValue);
    }
}