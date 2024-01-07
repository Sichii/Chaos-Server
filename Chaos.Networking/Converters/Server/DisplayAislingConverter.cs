using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="DisplayAislingArgs" /> into a buffer
/// </summary>
public sealed class DisplayAislingConverter : PacketConverterBase<DisplayAislingArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayAisling;

    /// <inheritdoc />
    public override DisplayAislingArgs Deserialize(ref SpanReader reader)
    {
        var point = reader.ReadPoint16();
        var direction = reader.ReadByte();
        var id = reader.ReadUInt32();

        var args = new DisplayAislingArgs
        {
            X = point.X,
            Y = point.Y,
            Direction = (Direction)direction,
            Id = id
        };

        var headSprite = reader.ReadUInt16();

        if (headSprite == ushort.MaxValue)
        {
            var sprite = reader.ReadUInt16();
            var headColor = reader.ReadByte();
            var bootsColor = reader.ReadByte();
            _ = reader.ReadBytes(6); //LI: what is this for? (maybe lanternSize, restPosition, isTransparent)?

            args.Sprite = (ushort)(sprite - NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET);
            args.HeadColor = (DisplayColor)headColor;
            args.BootsColor = (DisplayColor)bootsColor;
        } else
        {
            var bodySprite = reader.ReadByte();
            var armorSprite1 = reader.ReadUInt16();
            var bootsSprite = reader.ReadByte();
            var armorSprite2 = reader.ReadUInt16();
            var shieldSprite = reader.ReadByte();
            var weaponSprite = reader.ReadUInt16();
            var headColor = reader.ReadByte();
            var bootsColor = reader.ReadByte();
            var accessoryColor1 = reader.ReadByte();
            var accessorySprite1 = reader.ReadUInt16();
            var accessoryColor2 = reader.ReadByte();
            var accessorySprite2 = reader.ReadUInt16();
            var accessoryColor3 = reader.ReadByte();
            var accessorySprite3 = reader.ReadUInt16();
            var lanternSize = reader.ReadByte();
            var restPosition = reader.ReadByte();
            var overcoatSprite = reader.ReadUInt16();
            var overcoatColor = reader.ReadByte();
            var bodyColor = reader.ReadByte();
            var isTransparent = reader.ReadBoolean();
            var faceSprite = reader.ReadByte();

            args.HeadSprite = headSprite;

            var pantsColor = bodySprite % 16;

            if (pantsColor != 0)
            {
                bodySprite = (byte)(bodySprite - pantsColor);
                args.PantsColor = (DisplayColor)pantsColor;
            }

            args.BodySprite = (BodySprite)bodySprite;
            args.ArmorSprite1 = armorSprite1;
            args.BootsSprite = bootsSprite;
            args.ArmorSprite2 = armorSprite2;
            args.ShieldSprite = shieldSprite;
            args.WeaponSprite = weaponSprite;
            args.HeadColor = (DisplayColor)headColor;
            args.BootsColor = (DisplayColor)bootsColor;
            args.AccessoryColor1 = (DisplayColor)accessoryColor1;
            args.AccessorySprite1 = accessorySprite1;
            args.AccessoryColor2 = (DisplayColor)accessoryColor2;
            args.AccessorySprite2 = accessorySprite2;
            args.AccessoryColor3 = (DisplayColor)accessoryColor3;
            args.AccessorySprite3 = accessorySprite3;
            args.LanternSize = (LanternSize)lanternSize;
            args.RestPosition = (RestPosition)restPosition;
            args.OvercoatSprite = overcoatSprite;
            args.OvercoatColor = (DisplayColor)overcoatColor;
            args.BodyColor = (BodyColor)bodyColor;
            args.IsTransparent = isTransparent;
            args.FaceSprite = faceSprite;
        }

        var nameTagStyle = reader.ReadByte();
        var name = reader.ReadString8();
        var groupBoxText = reader.ReadString8();

        args.NameTagStyle = (NameTagStyle)nameTagStyle;
        args.Name = name;
        args.GroupBoxText = groupBoxText;

        if (args is { BodySprite: BodySprite.None, IsTransparent: true })
        {
            args.IsHidden = true;
            args.IsTransparent = false;
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayAislingArgs args)
    {
        writer.WritePoint16((ushort)args.X, (ushort)args.Y);
        writer.WriteByte((byte)args.Direction);
        writer.WriteUInt32(args.Id);

        if (args is { IsHidden: true })
        {
            writer.WriteUInt16(0);
            writer.WriteByte((byte)BodySprite.None);
            writer.WriteBytes(new byte[25]);
            writer.WriteBoolean(args.IsHidden);
            writer.WriteByte(0);
        } else if (args.Sprite.HasValue)
        {
            writer.WriteUInt16(ushort.MaxValue);
            writer.WriteUInt16((ushort)(args.Sprite.Value + NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET));
            writer.WriteByte((byte)args.HeadColor);
            writer.WriteByte((byte)args.BootsColor);
            writer.WriteBytes(new byte[6]);
        } else if (args.IsDead)
        {
            writer.WriteUInt16(args.HeadSprite);
            writer.WriteByte((byte)(args.Gender == Gender.Male ? BodySprite.MaleGhost : BodySprite.FemaleGhost));
            writer.WriteBytes(new byte[25]);
            writer.WriteBoolean(args.IsTransparent);
            writer.WriteByte(args.FaceSprite);
        } else
        {
            var pantsColor = (byte)(args.PantsColor ?? 0);
            var bodySprite = (byte)(args.BodySprite + pantsColor);

            writer.WriteUInt16(args.HeadSprite);
            writer.WriteByte(bodySprite);
            writer.WriteUInt16(args.ArmorSprite1);
            writer.WriteByte(args.BootsSprite);
            writer.WriteUInt16(args.ArmorSprite2);
            writer.WriteByte(args.ShieldSprite);
            writer.WriteUInt16(args.WeaponSprite);
            writer.WriteByte((byte)args.HeadColor);
            writer.WriteByte((byte)args.BootsColor);
            writer.WriteByte((byte)args.AccessoryColor1);
            writer.WriteUInt16(args.AccessorySprite1);
            writer.WriteByte((byte)args.AccessoryColor2);
            writer.WriteUInt16(args.AccessorySprite2);
            writer.WriteByte((byte)args.AccessoryColor3);
            writer.WriteUInt16(args.AccessorySprite3);
            writer.WriteByte((byte)args.LanternSize);
            writer.WriteByte((byte)args.RestPosition);
            writer.WriteUInt16(args.OvercoatSprite);
            writer.WriteByte((byte)args.OvercoatColor);
            writer.WriteByte((byte)args.BodyColor);
            writer.WriteBoolean(args.IsTransparent);
            writer.WriteByte(args.FaceSprite);
        }

        writer.WriteByte((byte)args.NameTagStyle);
        writer.WriteString8(args.Name);
        writer.WriteString8(args.GroupBoxText ?? string.Empty);
    }
}