using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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