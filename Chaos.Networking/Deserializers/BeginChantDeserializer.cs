using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record BeginChantDeserializer : ClientPacketDeserializer<BeginChantArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.BeginChant;

    public override BeginChantArgs Deserialize(ref SpanReader reader)
    {
        var castLineCount = reader.ReadByte();

        return new BeginChantArgs(castLineCount);
    }
}