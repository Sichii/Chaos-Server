using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="OtherProfileArgs" /> into a buffer
/// </summary>
public sealed class OtherProfileConverter : PacketConverterBase<OtherProfileArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.OtherProfile;

    /// <inheritdoc />
    public override OtherProfileArgs Deserialize(ref SpanReader reader)
    {
        var id = reader.ReadUInt32();

        //by default, all slots are populated with null
        var equipment = NETWORKING_CONSTANTS.PROFILE_EQUIPMENTSLOT_ORDER.ToDictionary(val => val, _ => default(ItemInfo));

        foreach (var slot in NETWORKING_CONSTANTS.PROFILE_EQUIPMENTSLOT_ORDER)
        {
            var sprite = reader.ReadUInt16();
            var color = reader.ReadByte();

            //only add the item if the sprite is not 0
            if (sprite is not 0)
            {
                sprite -= NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET;

                equipment.Add(
                    slot,
                    new ItemInfo
                    {
                        Sprite = sprite,
                        Color = (DisplayColor)color
                    });
            }
        }

        var socialStatus = reader.ReadByte();
        var name = reader.ReadString8();
        var nation = reader.ReadByte();
        var title = reader.ReadString8();
        var groupOpen = reader.ReadBoolean();
        var guildRank = reader.ReadString8();
        var displayClass = reader.ReadString8();
        var guildName = reader.ReadString8();
        var legendMarkCount = reader.ReadByte();
        var legendMarks = new List<LegendMarkInfo>(legendMarkCount);

        for (var i = 0; i < legendMarkCount; i++)
        {
            var icon = reader.ReadByte();
            var color = reader.ReadByte();
            var key = reader.ReadString8();
            var text = reader.ReadString8();

            legendMarks.Add(
                new LegendMarkInfo
                {
                    Icon = (MarkIcon)icon,
                    Color = (MarkColor)color,
                    Key = key,
                    Text = text
                });
        }

        var args = new OtherProfileArgs
        {
            Id = id,
            Equipment = equipment,
            SocialStatus = (SocialStatus)socialStatus,
            Name = name,
            Nation = (Nation)nation,
            Title = title,
            GroupOpen = groupOpen,
            GuildRank = guildRank,
            DisplayClass = displayClass,
            GuildName = guildName,
            LegendMarks = legendMarks
        };

        var remaining = reader.ReadUInt16();

        if (remaining == 0)
            return args;

        var portraitData = reader.ReadData16();
        var profileText = reader.ReadString16();

        args.Portrait = portraitData;
        args.ProfileText = profileText;

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, OtherProfileArgs args)
    {
        writer.WriteUInt32(args.Id);

        foreach (var slot in NETWORKING_CONSTANTS.PROFILE_EQUIPMENTSLOT_ORDER)
        {
            args.Equipment.TryGetValue(slot, out var item);

            var offsetSprite = item?.Sprite ?? 0;

            if (offsetSprite is not 0)
                offsetSprite += NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET;

            writer.WriteUInt16(offsetSprite);
            writer.WriteByte((byte)(item?.Color ?? DisplayColor.Default));
        }

        writer.WriteByte((byte)args.SocialStatus);
        writer.WriteString8(args.Name);
        writer.WriteByte((byte)args.Nation);
        writer.WriteString8(args.Title ?? string.Empty);
        writer.WriteBoolean(args.GroupOpen);
        writer.WriteString8(args.GuildRank ?? string.Empty);
        writer.WriteString8(args.DisplayClass);
        writer.WriteString8(args.GuildName ?? string.Empty);
        writer.WriteByte((byte)args.LegendMarks.Count);

        foreach (var mark in args.LegendMarks)
        {
            writer.WriteByte((byte)mark.Icon);
            writer.WriteByte((byte)mark.Color);
            writer.WriteString8(mark.Key);
            writer.WriteString8(mark.Text);
        }

        var remaining = args.Portrait.Length;
        remaining += args.ProfileText?.Length ?? 0;

        //if theres no portrait or profile data, just write 0
        if (remaining == 0)
            writer.WriteUInt16(0);
        else //if there's data, write the length of the data + 4 for prefixes
        {
            writer.WriteUInt16((ushort)(remaining + 4));
            writer.WriteData16(args.Portrait); //2 + length
            writer.WriteString16(args.ProfileText ?? string.Empty); //2 + length
        }

        //nfi
        writer.WriteByte(0);
    }
}