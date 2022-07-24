using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

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