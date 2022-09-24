using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record SetNotepadDeserializer : ClientPacketDeserializer<SetNotepadArgs>
{
    public override ClientOpCode ClientOpCode { get; } = ClientOpCode.SetNotepad;

    public override SetNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var message = reader.ReadString16();

        return new SetNotepadArgs(slot, message);
    }
}