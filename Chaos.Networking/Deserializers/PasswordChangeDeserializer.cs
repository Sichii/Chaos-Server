using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record PasswordChangeDeserializer : ClientPacketDeserializer<PasswordChangeArgs>
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