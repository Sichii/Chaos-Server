using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="MenuArgs" /> into a buffer
/// </summary>
public sealed class MenuConverter : PacketConverterBase<MenuArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Menu;

    /// <inheritdoc />
    public override MenuArgs Deserialize(ref SpanReader reader)
    {
        var menuType = reader.ReadByte();
        var entityType = reader.ReadByte();
        var sourceId = reader.ReadUInt32();
        _ = reader.ReadByte(); //LI: what is this for?
        var sprite = reader.ReadUInt16();
        var color = reader.ReadByte();
        _ = reader.ReadByte(); //LI: what is this for?
        var sprite2 = reader.ReadUInt16();
        var color2 = reader.ReadByte();
        var shouldIllustrate = reader.ReadBoolean();
        var name = reader.ReadString8();
        var text = reader.ReadString16();

        if (sprite == 0)
            sprite = sprite2;

        if (color == 0)
            color = color2;

        switch (sprite)
        {
            case > NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET:
                sprite -= NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET;

                break;
            case > NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET:
                sprite -= NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET;

                break;
        }

        var menuArgs = new MenuArgs
        {
            MenuType = (MenuType)menuType,
            EntityType = (EntityType)entityType,
            SourceId = sourceId,
            Sprite = sprite,
            Color = (DisplayColor)color,
            Name = name,
            Text = text,
            ShouldIllustrate = shouldIllustrate
        };

        switch (menuArgs.MenuType)
        {
            case MenuType.Menu:
            {
                var optionCount = reader.ReadByte();
                var options = new List<(string Text, ushort Pursuit)>(optionCount);

                for (var i = 0; i < optionCount; i++)
                {
                    var optionText = reader.ReadString8();
                    var optionPursuit = reader.ReadUInt16();

                    options.Add((optionText, optionPursuit));
                }

                menuArgs.Options = options;

                break;
            }
            case MenuType.MenuWithArgs:
            {
                var args = reader.ReadString8();
                var optionCount = reader.ReadByte();
                var options = new List<(string Text, ushort Pursuit)>(optionCount);

                for (var i = 0; i < optionCount; i++)
                {
                    var optionText = reader.ReadString8();
                    var optionPursuit = reader.ReadUInt16();

                    options.Add((optionText, optionPursuit));
                }

                menuArgs.Args = args;
                menuArgs.Options = options;

                break;
            }
            case MenuType.TextEntry:
            {
                var pursuitId = reader.ReadUInt16();

                menuArgs.PursuitId = pursuitId;

                break;
            }
            case MenuType.TextEntryWithArgs:
            {
                var args = reader.ReadString8();
                var pursuitId = reader.ReadUInt16();

                menuArgs.Args = args;
                menuArgs.PursuitId = pursuitId;

                break;
            }
            case MenuType.ShowItems:
            {
                var pursuitId = reader.ReadUInt16();
                var itemCount = reader.ReadUInt16();
                var items = new List<ItemInfo>(itemCount);

                for (var i = 0; i < itemCount; i++)
                {
                    var itemSprite = reader.ReadUInt16();
                    var itemColor = reader.ReadByte();
                    var cost = reader.ReadInt32();
                    var itemName = reader.ReadString8();
                    _ = reader.ReadString8(); //LI: what is this for?

                    items.Add(
                        new ItemInfo
                        {
                            Sprite = (ushort)(itemSprite - NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET),
                            Color = (DisplayColor)itemColor,
                            Cost = cost,
                            Name = itemName
                        });
                }

                menuArgs.PursuitId = pursuitId;
                menuArgs.Items = items;

                break;
            }
            case MenuType.ShowPlayerItems:
            {
                var pursuitId = reader.ReadUInt16();
                var slotCount = reader.ReadByte();
                var slots = new List<byte>(slotCount);

                for (var i = 0; i < slotCount; i++)
                    slots.Add(reader.ReadByte());

                menuArgs.PursuitId = pursuitId;
                menuArgs.Slots = slots;

                break;
            }
            case MenuType.ShowSpells:
            {
                var pursuitId = reader.ReadUInt16();
                var spellCount = reader.ReadUInt16();
                var spells = new List<SpellInfo>(spellCount);

                for (var i = 0; i < spellCount; i++)
                {
                    _ = reader.ReadByte(); //EntityType (see below)
                    var icon = reader.ReadUInt16();
                    _ = reader.ReadByte(); //color if entityType is item
                    var spellName = reader.ReadString8();

                    spells.Add(
                        new SpellInfo
                        {
                            Sprite = icon,
                            Name = spellName
                        });
                }

                menuArgs.PursuitId = pursuitId;
                menuArgs.Spells = spells;

                break;
            }
            case MenuType.ShowSkills:
            {
                var pursuitId = reader.ReadUInt16();
                var skillCount = reader.ReadUInt16();
                var skills = new List<SkillInfo>(skillCount);

                for (var i = 0; i < skillCount; i++)
                {
                    _ = reader.ReadByte(); //EntityType (see below)
                    var icon = reader.ReadUInt16();
                    _ = reader.ReadByte(); //color if entityType is item
                    var skillName = reader.ReadString8();

                    skills.Add(
                        new SkillInfo
                        {
                            Sprite = icon,
                            Name = skillName
                        });
                }

                menuArgs.PursuitId = pursuitId;
                menuArgs.Skills = skills;

                break;
            }
            case MenuType.ShowPlayerSpells:
            case MenuType.ShowPlayerSkills:
            {
                var pursuitId = reader.ReadUInt16();

                menuArgs.PursuitId = pursuitId;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(menuArgs.MenuType),
                    "Encountered unknown menu type value during deserialization");
        }

        return menuArgs;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MenuArgs args)
    {
        var offsetSprite = args.Sprite;

        if (args.Sprite is not 0)
            switch (args.EntityType)
            {
                case EntityType.Item:
                    offsetSprite += NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET;

                    break;
                case EntityType.Aisling or EntityType.Creature:
                    offsetSprite += NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET;

                    break;
            }

        writer.WriteByte((byte)args.MenuType);
        writer.WriteByte((byte)args.EntityType);
        writer.WriteUInt32(args.SourceId ?? 0);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(offsetSprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(offsetSprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteBoolean(args.ShouldIllustrate);
        writer.WriteString8(args.Name);
        writer.WriteString16(args.Text);

        switch (args.MenuType)
        {
            case MenuType.Menu:
            {
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var option in args.Options)
                {
                    writer.WriteString8(option.Text);
                    writer.WriteUInt16(option.Pursuit);
                }

                break;
            }
            case MenuType.MenuWithArgs:
            {
                writer.WriteString8(args.Args!);
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var option in args.Options)
                {
                    writer.WriteString8(option.Text);
                    writer.WriteUInt16(option.Pursuit);
                }

                break;
            }
            case MenuType.TextEntry:
                writer.WriteUInt16(args.PursuitId);

                break;
            case MenuType.TextEntryWithArgs:
                writer.WriteString8(args.Args!);
                writer.WriteUInt16(args.PursuitId);

                break;

            case MenuType.ShowItems:
                writer.WriteUInt16(args.PursuitId);
                writer.WriteUInt16((ushort)args.Items!.Count);

                foreach (var item in args.Items)
                {
                    writer.WriteUInt16((ushort)(item.Sprite + NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET));
                    writer.WriteByte((byte)item.Color);
                    writer.WriteUInt32((uint)item.Cost!.Value);
                    writer.WriteString8(item.Name);

                    //TODO: figure out what this is, maybe something to do with metadatas
                    writer.WriteString8("what is this");
                }

                break;
            case MenuType.ShowPlayerItems:
                writer.WriteUInt16(args.PursuitId);
                writer.WriteByte((byte)args.Slots!.Count);

                foreach (var slot in args.Slots)
                    writer.WriteByte(slot);

                break;
            case MenuType.ShowSpells:
                writer.WriteUInt16(args.PursuitId);
                writer.WriteUInt16((ushort)args.Spells!.Count);

                foreach (var spell in args.Spells)
                {
                    //0 = none
                    //1 = item (requires offset sprite)
                    //2 = spell icon
                    //3 = skill icon
                    //4 = monster sprite (requires offset sprite).. theyre all facing up?
                    writer.WriteByte(2);
                    writer.WriteUInt16(spell.Sprite);
                    writer.WriteByte(0); //color
                    writer.WriteString8(spell.Name);
                }

                break;
            case MenuType.ShowSkills:
                writer.WriteUInt16((ushort)(args.PursuitId + 1));
                writer.WriteUInt16((ushort)args.Skills!.Count);

                foreach (var skill in args.Skills)
                {
                    writer.WriteByte(3);
                    writer.WriteUInt16(skill.Sprite);
                    writer.WriteByte(0); //color
                    writer.WriteString8(skill.Name);
                }

                break;
            case MenuType.ShowPlayerSpells:
                writer.WriteUInt16(args.PursuitId);

                break;
            case MenuType.ShowPlayerSkills:
                writer.WriteUInt16(args.PursuitId);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.MenuType), args.MenuType, "Unknown menu type");
        }
    }
}