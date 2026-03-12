#region
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayReadonlyNotepadArgs" />
/// </summary>
public sealed class DisplayReadonlyNotepadConverter : PacketConverterBase<DisplayReadonlyNotepadArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayReadonlyNotepad;

    /// <inheritdoc />
    public override DisplayReadonlyNotepadArgs Deserialize(ref SpanReader reader)
    {
        var type = reader.ReadByte();
        var width = reader.ReadByte();
        var height = reader.ReadByte();

        // ReSharper disable once UnusedVariable
        var unknown = reader.ReadByte();
        var message = reader.ReadString16();

        return new DisplayReadonlyNotepadArgs
        {
            NotepadType = (NotepadType)type,
            Height = height,
            Width = width,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayReadonlyNotepadArgs args)
    {
        writer.WriteByte((byte)args.NotepadType);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteByte(0); //always zero
        writer.WriteString16(args.Message);
    }
}