using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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