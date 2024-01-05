using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="DisplayVisibleEntitiesArgs" /> into a buffer
/// </summary>
public sealed class DisplayVisibleEntitiesConverter : PacketConverterBase<DisplayVisibleEntitiesArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayVisibleEntities;

    /// <inheritdoc />
    public override DisplayVisibleEntitiesArgs Deserialize(ref SpanReader reader)
    {
        var count = reader.ReadUInt16();
        var visibleObjects = new List<VisibleEntityInfo>(count);

        for (var i = 0; i < count; i++)
        {
            var point = reader.ReadPoint16();
            var id = reader.ReadUInt32();
            var sprite = reader.ReadUInt16();

            switch (sprite)
            {
                case >= NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET:
                {
                    var color = reader.ReadByte();

                    visibleObjects.Add(
                        new GroundItemInfo
                        {
                            X = point.X,
                            Y = point.Y,
                            Id = id,
                            Sprite = (ushort)(sprite - NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET),
                            Color = (DisplayColor)color
                        });

                    break;
                }
                case >= NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET:
                {
                    _ = reader.ReadBytes(4); //LI: what is this for?
                    var direction = reader.ReadByte();
                    _ = reader.ReadByte(); //LI: what is this for?
                    var creatureType = reader.ReadByte();

                    var creatureInfo = new CreatureInfo
                    {
                        X = point.X,
                        Y = point.Y,
                        Id = id,
                        Sprite = (ushort)(sprite - NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET),
                        Direction = (Direction)direction,
                        CreatureType = (CreatureType)creatureType
                    };

                    if (creatureInfo.CreatureType == CreatureType.Merchant)
                        creatureInfo.Name = reader.ReadString8();

                    visibleObjects.Add(creatureInfo);

                    break;
                }
            }
        }

        return new DisplayVisibleEntitiesArgs
        {
            VisibleObjects = visibleObjects
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayVisibleEntitiesArgs args)
    {
        writer.WriteUInt16((ushort)args.VisibleObjects.Count);

        foreach (var obj in args.VisibleObjects)
        {
            writer.WritePoint16((ushort)obj.X, (ushort)obj.Y);
            writer.WriteUInt32(obj.Id);

            switch (obj)
            {
                case CreatureInfo creature:
                {
                    writer.WriteUInt16((ushort)(obj.Sprite + NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET));
                    writer.WriteBytes(new byte[4]); //LI: what is this for?
                    writer.WriteByte((byte)creature.Direction);
                    writer.WriteByte(0); //LI: what is this for?
                    writer.WriteByte((byte)creature.CreatureType);

                    if (creature.CreatureType == CreatureType.Merchant)
                        writer.WriteString8(creature.Name);

                    break;
                }
                case GroundItemInfo groundItem:
                    writer.WriteUInt16((ushort)(obj.Sprite + NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET));
                    writer.WriteByte((byte)groundItem.Color);
                    writer.WriteBytes(new byte[2]);

                    break;
            }
        }
    }
}