using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="NotepadArgs" /> into a buffer
/// </summary>
public sealed class NotepadConverter : PacketConverterBase<NotepadArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Notepad;

    /// <inheritdoc />
    public override NotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var type = reader.ReadByte();
        var height = reader.ReadByte();
        var width = reader.ReadByte();
        var message = reader.ReadString16();

        return new NotepadArgs
        {
            Slot = slot,
            NotepadType = (NotepadType)type,
            Height = height,
            Width = width,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, NotepadArgs args)
    {
        writer.WriteByte(args.Slot);
        writer.WriteByte((byte)args.NotepadType);
        writer.WriteByte(args.Height);
        writer.WriteByte(args.Width);
        writer.WriteString16(args.Message);
    }
}