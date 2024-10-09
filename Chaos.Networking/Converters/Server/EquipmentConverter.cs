using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="EquipmentArgs" />
/// </summary>
public sealed class EquipmentConverter : PacketConverterBase<EquipmentArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Equipment;

    /// <inheritdoc />
    public override EquipmentArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var sprite = reader.ReadUInt16();
        var color = reader.ReadByte();
        var name = reader.ReadString8();
        _ = reader.ReadByte(); //LI: what is this for?
        var maxDurability = reader.ReadUInt32();
        var currentDurability = reader.ReadUInt32();

        return new EquipmentArgs
        {
            Slot = (EquipmentSlot)slot,
            Item = new ItemInfo
            {
                Sprite = (ushort)(sprite - NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET),
                Color = (DisplayColor)color,
                Name = name,
                MaxDurability = (int)maxDurability,
                CurrentDurability = (int)currentDurability,
                Slot = slot
            }
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, EquipmentArgs args)
    {
        writer.WriteByte((byte)args.Slot);
        writer.WriteUInt16((ushort)(args.Item.Sprite + NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET));
        writer.WriteByte((byte)args.Item.Color);
        writer.WriteString8(args.Item.Name);
        writer.WriteByte(0); //LI: what is this for?
        writer.WriteUInt32((uint)args.Item.MaxDurability);
        writer.WriteUInt32((uint)args.Item.CurrentDurability);
    }
}