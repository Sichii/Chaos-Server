using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record NotepadSerializer : ServerPacketSerializer<NotepadArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Notepad;

    public override void Serialize(ref SpanWriter writer, NotepadArgs args)
    {
        writer.WriteByte(args.Slot);
        writer.WriteByte((byte)args.NotepadType);
        writer.WriteByte(args.Height);
        writer.WriteByte(args.Width);
        writer.WriteString16(args.Message);
    }
}