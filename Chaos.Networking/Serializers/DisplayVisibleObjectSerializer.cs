using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record DisplayVisibleObjectSerializer : ServerPacketSerializer<DisplayVisibleObjectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.DisplayVisibleObject;

    public override void Serialize(ref SpanWriter writer, DisplayVisibleObjectArgs args)
    {
        writer.WriteUInt16((ushort)args.VisibleObjects.Count);

        foreach (var obj in args.VisibleObjects)
        {
            writer.WritePoint16((ushort)obj.X, (ushort)obj.Y);
            writer.WriteUInt32(obj.Id);
            writer.WriteUInt16(obj.Sprite);

            if (obj is CreatureInfo creature)
            {
                writer.WriteBytes(new byte[4]); //dunno
                writer.WriteByte((byte)creature.Direction);
                writer.WriteByte(0); //dunno
                writer.WriteByte((byte)creature.CreatureType);

                if (creature.CreatureType == CreatureType.Merchant)
                    writer.WriteString8(creature.Name);
            } else if (obj is GroundItemInfo groundItem)
            {
                writer.WriteByte((byte)groundItem.Color);
                writer.WriteBytes(new byte[2]);
            }
        }
    }
}