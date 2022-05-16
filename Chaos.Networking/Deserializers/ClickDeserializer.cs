using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record ClickDeserializer : ClientPacketDeserializer<ClickArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Click;

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
            case ClickType.Unknown:
            //i have no idea, coords are in here somehow
            default:
                throw new ArgumentOutOfRangeException(nameof(clickType), clickType, "Unknown enum value");
        }

        return new ClickArgs(targetId, targetPoint);
    }
}