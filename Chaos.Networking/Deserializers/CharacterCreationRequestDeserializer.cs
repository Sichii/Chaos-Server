using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record CharacterCreationRequestDeserializer : ClientPacketDeserializer<CreateCharRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.CreateCharRequest;

    public override CreateCharRequestArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new CreateCharRequestArgs(name, pw);
    }
}