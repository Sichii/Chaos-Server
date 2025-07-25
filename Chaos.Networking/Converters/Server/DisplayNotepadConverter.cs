#region
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayNotepadArgs" />
/// </summary>
public sealed class DisplayNotepadConverter : PacketConverterBase<DisplayNotepadArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayNotepad;

    /// <inheritdoc />
    public override DisplayNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var type = reader.ReadByte();
        var height = reader.ReadByte();
        var width = reader.ReadByte();
        var message = reader.ReadString16();

        return new DisplayNotepadArgs
        {
            Slot = slot,
            NotepadType = (NotepadType)type,
            Height = height,
            Width = width,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayNotepadArgs args)
    {
        writer.WriteByte(args.Slot);
        writer.WriteByte((byte)args.NotepadType);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteString16(args.Message);
    }
}