using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record PasswordChangeDeserializer : ClientPacketDeserializer<PasswordChangeArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.PasswordChange;

    public override PasswordChangeArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var currentPw = reader.ReadString8();
        var newPw = reader.ReadString8();

        return new PasswordChangeArgs(name, currentPw, newPw);
    }
}