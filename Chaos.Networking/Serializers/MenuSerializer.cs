using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record MenuSerializer : ServerPacketSerializer<MenuArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Menu;

    public override void Serialize(ref SpanWriter writer, MenuArgs args)
    {
        writer.WriteByte((byte)args.MenuType);
        writer.WriteByte((byte)args.GameObjectType);
        writer.WriteUInt32(args.SourceId ?? 0);
        writer.WriteByte(1); //dunno
        writer.WriteUInt16(args.Sprite);
        writer.WriteByte(0);
        writer.WriteByte(1);
        writer.WriteUInt16(args.Sprite);
        writer.WriteBytes(new byte[2]);
        writer.WriteString8(args.Name);
        writer.WriteString16(args.Text);

        switch (args.MenuType)
        {
            case MenuType.Menu:
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var pursuit in args.Options)
                {
                    writer.WriteString8(pursuit.ToString());
                    writer.WriteUInt16(pursuit);
                }

                break;
            case MenuType.MenuWithArgs:
                writer.WriteString8(args.Args!);
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var pursuit in args.Options)
                {
                    writer.WriteString8(pursuit.ToString());
                    writer.WriteUInt16(pursuit);
                }

                break;
            case MenuType.TextEntry:
                writer.WriteUInt16((ushort)args.PursuitId!);

                break;
            case MenuType.ShowItems:
                writer.WriteUInt16((ushort)args.PursuitId!);
                writer.WriteUInt16((ushort)args.Items!.Count);

                foreach (var item in args.Items)
                {
                    writer.WriteUInt16(item.Sprite);
                    writer.WriteByte((byte)item.Color);
                    writer.WriteUInt32((uint)item.Cost!.Value);
                    writer.WriteString8(item.Name);
                    writer.WriteString8("what is this");
                }

                break;
            case MenuType.ShowOwnedItems:
                writer.WriteUInt16((ushort)args.PursuitId!);
                writer.WriteUInt16((ushort)args.Slots!.Count);

                foreach (var slot in args.Slots)
                    writer.WriteByte(slot);

                break;
            case MenuType.ShowSpells:
                writer.WriteUInt16((ushort)args.PursuitId!);
                writer.WriteUInt16((ushort)args.Spells!.Count);

                foreach (var spell in args.Spells)
                {
                    writer.WriteByte(2);
                    writer.WriteUInt16(spell.Sprite);
                    writer.WriteByte(0); //idk
                    writer.WriteString8(spell.Name);
                }

                break;
            case MenuType.ShowSkills:
                writer.WriteUInt16((ushort)args.PursuitId!);
                writer.WriteUInt16((ushort)args.Skills!.Count);

                foreach (var skill in args.Skills)
                {
                    writer.WriteByte(3);
                    writer.WriteUInt16(skill.Sprite);
                    writer.WriteByte(0); //idk
                    writer.WriteString8(skill.Name);
                }

                break;
            case MenuType.ShowLearnedSpells:
                writer.WriteUInt16((ushort)args.PursuitId!);

                break;
            case MenuType.ShowLearnedSkills:
                writer.WriteUInt16((ushort)args.PursuitId!);

                break;
            case MenuType.Dialog:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.MenuType), args.MenuType, "Unknown menu type");
        }
    }
}