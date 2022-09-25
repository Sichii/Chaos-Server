using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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