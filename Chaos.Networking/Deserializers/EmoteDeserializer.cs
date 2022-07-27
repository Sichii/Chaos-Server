using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

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