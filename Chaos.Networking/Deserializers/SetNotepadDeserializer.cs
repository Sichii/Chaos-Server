using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record SetNotepadDeserializer : ClientPacketDeserializer<SetNotepadArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SetNotepad;

    public override SetNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var message = reader.ReadString16();

        return new SetNotepadArgs(slot, message);
    }
}