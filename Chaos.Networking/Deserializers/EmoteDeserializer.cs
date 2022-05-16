using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record EmoteDeserializer : ClientPacketDeserializer<EmoteArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Emote;

    public override EmoteArgs Deserialize(ref SpanReader reader)
    {
        var bodyAnimation = (BodyAnimation)(reader.ReadByte() + 9);

        return new EmoteArgs(bodyAnimation);
    }
}