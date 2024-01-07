using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="DialogArgs" /> into a buffer
/// </summary>
public sealed class DialogConverter : PacketConverterBase<DialogArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Dialog;

    /// <inheritdoc />
    public override DialogArgs Deserialize(ref SpanReader reader)
    {
        var dialogType = reader.ReadByte();

        if (dialogType == (byte)DialogType.CloseDialog)
            return new DialogArgs
            {
                DialogType = (DialogType)dialogType
            };

        var entityType = reader.ReadByte();
        var sourceId = reader.ReadUInt32();
        _ = reader.ReadByte(); //dunno
        var sprite = reader.ReadUInt16();
        var color = reader.ReadByte();
        _ = reader.ReadByte(); //dunno
        var sprite2 = reader.ReadUInt16();
        var color2 = reader.ReadByte();
        var pursuitId = reader.ReadUInt16();
        var dialogId = reader.ReadUInt16();
        var hasPreviousButton = reader.ReadBoolean();
        var hasNextButton = reader.ReadBoolean();
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

        var args = new DialogArgs
        {
            DialogType = (DialogType)dialogType,
            EntityType = (EntityType)entityType,
            SourceId = sourceId,
            Sprite = sprite,
            Color = (DisplayColor)color,
            PursuitId = pursuitId,
            DialogId = dialogId,
            HasPreviousButton = hasPreviousButton,
            HasNextButton = hasNextButton,
            ShouldIllustrate = shouldIllustrate,
            Name = name,
            Text = text
        };

        switch (args.DialogType)
        {
            case DialogType.Normal:
                break;
            case DialogType.DialogMenu:
            {
                var optionsCount = reader.ReadByte();
                var options = new List<string>(optionsCount);

                for (var i = 0; i < optionsCount; i++)
                {
                    var option = reader.ReadString8();

                    options.Add(option);
                }

                args.Options = options;

                break;
            }
            case DialogType.TextEntry:
            {
                var textBoxPrompt = reader.ReadString8();
                var textBoxLength = reader.ReadByte();

                args.TextBoxPrompt = textBoxPrompt;
                args.TextBoxLength = textBoxLength;

                break;
            }
            case DialogType.Speak:
                break;
            case DialogType.CreatureMenu:
            {
                var optionsCount = reader.ReadByte();
                var options = new List<string>(optionsCount);

                for (var i = 0; i < optionsCount; i++)
                {
                    var option = reader.ReadString8();

                    options.Add(option);
                }

                args.Options = options;

                break;
            }
            case DialogType.Protected:
                break;
            case DialogType.CloseDialog:
                throw new InvalidOperationException("This should never happen, CloseDialog is handled above");
            default:
                throw new ArgumentOutOfRangeException(nameof(args.DialogType), args.DialogType, "Unknown dialog type");
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DialogArgs args)
    {
        writer.WriteByte((byte)args.DialogType);

        if (args.DialogType == DialogType.CloseDialog)
            return;

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

        writer.WriteByte((byte)args.EntityType);
        writer.WriteUInt32(args.SourceId ?? 0);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(offsetSprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteByte(0); //dunno
        writer.WriteUInt16(offsetSprite);
        writer.WriteByte((byte)args.Color);
        writer.WriteUInt16(args.PursuitId ?? 0);
        writer.WriteUInt16(args.DialogId);
        writer.WriteBoolean(args.HasPreviousButton);
        writer.WriteBoolean(args.HasNextButton);
        writer.WriteBoolean(args.ShouldIllustrate);
        writer.WriteString8(args.Name);
        writer.WriteString16(args.Text);

        switch (args.DialogType)
        {
            case DialogType.Normal:
                break;
            case DialogType.DialogMenu:
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var option in args.Options)
                    writer.WriteString8(option);

                break;
            case DialogType.TextEntry:
                writer.WriteString8(args.TextBoxPrompt ?? string.Empty);
                writer.WriteByte((byte)(args.TextBoxLength ?? 0));

                break;
            case DialogType.Speak:
                break;
            case DialogType.CreatureMenu:
                writer.WriteByte((byte)args.Options!.Count);

                foreach (var option in args.Options)
                    writer.WriteString8(option);

                break;
            case DialogType.Protected:
                break;
            case DialogType.CloseDialog:
                throw new InvalidOperationException("This should never happen, CloseDialog is handled above");
            default:
                throw new ArgumentOutOfRangeException(nameof(args.DialogType), args.DialogType, "Unknown dialog type");
        }
    }
}