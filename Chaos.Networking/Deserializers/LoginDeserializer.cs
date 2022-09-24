using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record LoginDeserializer : ClientPacketDeserializer<LoginArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Login;

    public override LoginArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new LoginArgs(name, pw);
    }
}