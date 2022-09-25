using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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