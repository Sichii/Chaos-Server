using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record ChantDeserializer : ClientPacketDeserializer<DisplayChantArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Chant;

    public override DisplayChantArgs Deserialize(ref SpanReader reader)
    {
        var chantMessage = reader.ReadString8();

        return new DisplayChantArgs(chantMessage);
    }
}