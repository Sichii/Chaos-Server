#region
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayEditableNotepadArgs" />
/// </summary>
public sealed class DisplayEditableNotepadConverter : PacketConverterBase<DisplayEditableNotepadArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayEditableNotepad;

    /// <inheritdoc />
    public override DisplayEditableNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var type = reader.ReadByte();
        var width = reader.ReadByte();
        var height = reader.ReadByte();
        var message = reader.ReadString16();

        return new DisplayEditableNotepadArgs
        {
            Slot = slot,
            NotepadType = (NotepadType)type,
            Height = height,
            Width = width,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayEditableNotepadArgs args)
    {
        writer.WriteByte(args.Slot);
        writer.WriteByte((byte)args.NotepadType);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteString16(args.Message);
    }
}