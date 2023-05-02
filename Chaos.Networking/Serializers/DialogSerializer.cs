using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="DialogArgs" /> into a buffer
/// </summary>
public sealed record DialogSerializer : ServerPacketSerializer<DialogArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Dialog;

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
        writer.WriteByte(0); //illustration frame index, but none of the current images have multiple frames
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
                writer.WriteUInt16(args.TextBoxLength!.Value);

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
                throw new InvalidOperationException("This should never happen");
            default:
                throw new ArgumentOutOfRangeException(nameof(args.DialogType), args.DialogType, "Unknown dialog type");
        }
    }
}