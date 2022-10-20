using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record AddItemToPaneSerializer : ServerPacketSerializer<AddItemToPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.AddItemToPane;

    public override void Serialize(ref SpanWriter writer, AddItemToPaneArgs args)
    {
        writer.WriteByte(args.Item.Slot);
        writer.WriteUInt16(args.Item.Sprite);
        writer.WriteByte((byte)args.Item.Color);
        writer.WriteString8(args.Item.Name);
        writer.WriteUInt32(args.Item.Count!.Value);
        writer.WriteBoolean(args.Item.Stackable);
        writer.WriteUInt32((uint)args.Item.MaxDurability);
        writer.WriteUInt32((uint)args.Item.CurrentDurability);

        //nfi
        if (args.Item.Stackable)
            writer.WriteByte(0);
    }
}