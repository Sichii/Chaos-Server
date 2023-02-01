using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record MenuSerializer : ServerPacketSerializer<MenuArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Menu;

    public override void Serialize(ref SpanWriter writer, MenuArgs args)
    {
        writer.WriteByte((byte)args.MenuType);
        writer.WriteByte((byte)args.EntityType);
        writer.WriteUInt32(args.SourceId ?? 0);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(args.Sprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(args.Sprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteByte(0); //illustration frame index, but none of the current images have multiple frames
        writer.WriteString8(args.Name);
        writer.WriteString16(args.Text);

        switch (args.MenuType)
        {
            case MenuType.Menu:
            {
                writer.WriteByte((byte)args.Options!.Count);
                ushort index = 1;

                foreach (var option in args.Options)
                {
                    writer.WriteString8(option);
                    writer.WriteUInt16(index++);
                }

                break;
            }
            case MenuType.MenuWithArgs:
            {
                writer.WriteString8(args.Args!);
                writer.WriteByte((byte)args.Options!.Count);
                ushort index = 1;

                foreach (var option in args.Options)
                {
                    writer.WriteString8(option);
                    writer.WriteUInt16(index++);
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
                    writer.WriteUInt16(item.Sprite);
                    writer.WriteByte((byte)item.Color);
                    writer.WriteUInt32((uint)item.Cost!.Value);
                    writer.WriteString8(item.Name);
                    //TODO: figure out what this is, maybe something to do with metafiles
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