using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ClickArgs" />
/// </summary>
public sealed class ClickConverter : PacketConverterBase<ClickArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Click;

    /// <inheritdoc />
    public override ClickArgs Deserialize(ref SpanReader reader)
    {
        var clickType = reader.ReadByte();

        var args = new ClickArgs
        {
            ClickType = (ClickType)clickType
        };

        switch (args.ClickType)
        {
            case ClickType.TargetId:
                args.TargetId = reader.ReadUInt32();

                break;
            case ClickType.TargetPoint:
                args.TargetPoint = (Point)reader.ReadPoint16();

                break;

            //i have no idea, coords are in here somehow
            default:
                throw new ArgumentOutOfRangeException(nameof(clickType), clickType, "Unknown enum value");
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ClickArgs args)
    {
        writer.WriteByte((byte)args.ClickType);

        switch (args.ClickType)
        {
            case ClickType.TargetId:
                writer.WriteUInt32(args.TargetId!.Value);

                break;
            case ClickType.TargetPoint:
                writer.WritePoint16(args.TargetPoint!);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.ClickType), args.ClickType, "Unknown enum value");
        }
    }
}