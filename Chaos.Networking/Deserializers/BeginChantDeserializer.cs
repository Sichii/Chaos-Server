using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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