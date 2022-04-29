using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record DialogSerializer : ServerPacketSerializer<DialogArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Dialog;

    public override void Serialize(ref SpanWriter writer, DialogArgs args)
    {
        writer.WriteByte((byte)args.DialogType);

        if (args.DialogType == DialogType.CloseDialog)
            return;

        writer.WriteUInt32(args.SourceId ?? 0);
        writer.WriteByte(0);
        writer.WriteUInt16(args.Sprite);
        writer.WriteBytes(new byte[2]);
        writer.WriteUInt16(args.Sprite);
        writer.WriteByte(0);
        writer.WriteUInt16((ushort)(args.PursuitId ?? 0));
        writer.WriteUInt16(args.DialogId);
        writer.WriteBoolean(args.HasPreviousButton);
        writer.WriteBoolean(args.HasNextButton);
        writer.WriteByte(0);
        writer.WriteString8(args.Name);
        writer.WriteString16(args.Text);

        switch (args.DialogType)
        {
            case DialogType.Normal:
                break;
            case DialogType.ItemMenu:
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