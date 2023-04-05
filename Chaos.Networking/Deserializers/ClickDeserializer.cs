using Chaos.Common.Definitions;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ClickArgs" />
/// </summary>
public sealed record ClickDeserializer : ClientPacketDeserializer<ClickArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Click;

    /// <inheritdoc />
    public override ClickArgs Deserialize(ref SpanReader reader)
    {
        var clickType = (ClickType)reader.ReadByte();
        var targetId = default(uint?);
        var targetPoint = default(Point?);

        switch (clickType)
        {
            case ClickType.TargetId:
                targetId = reader.ReadUInt32();

                break;
            case ClickType.TargetPoint:
                targetPoint = reader.ReadPoint16();

                break;
            //i have no idea, coords are in here somehow
            default:
                throw new ArgumentOutOfRangeException(nameof(clickType), clickType, "Unknown enum value");
        }

        return new ClickArgs(targetId, targetPoint);
    }
}