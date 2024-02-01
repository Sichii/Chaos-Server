using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SetNotepadArgs" />
/// </summary>
public sealed class SetNotepadConverter : PacketConverterBase<SetNotepadArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SetNotepad;

    /// <inheritdoc />
    public override SetNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var message = reader.ReadString16();

        return new SetNotepadArgs
        {
            Slot = slot,
            Message = message
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SetNotepadArgs args)
    {
        writer.WriteByte(args.Slot);
        writer.WriteString16(args.Message);
    }
}