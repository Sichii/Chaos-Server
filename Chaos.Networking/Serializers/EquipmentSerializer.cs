using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record EquipmentSerializer : ServerPacketSerializer<EquipmentArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Equipment;

    public override void Serialize(ref SpanWriter writer, EquipmentArgs args)
    {
        writer.WriteByte((byte)args.Slot);
        writer.WriteUInt16(args.Item.Sprite);
        writer.WriteByte((byte)args.Item.Color);
        writer.WriteString8(args.Item.Name);
        writer.WriteByte(0); //dunno
        writer.WriteUInt32((uint)args.Item.MaxDurability);
        writer.WriteUInt32((uint)args.Item.CurrentDurability);
    }
}