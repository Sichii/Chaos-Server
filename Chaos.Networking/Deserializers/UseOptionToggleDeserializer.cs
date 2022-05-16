using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record UserOptionToggleDeserializer : ClientPacketDeserializer<UserOptionToggleArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.UserOptionToggle;

    public override UserOptionToggleArgs Deserialize(ref SpanReader reader)
    {
        var userOption = (UserOption)reader.ReadByte();

        return new UserOptionToggleArgs(userOption);
    }
}