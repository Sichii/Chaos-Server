using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="UserOptionToggleArgs" />
/// </summary>
public sealed record UserOptionToggleDeserializer : ClientPacketDeserializer<UserOptionToggleArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.UserOptionToggle;

    /// <inheritdoc />
    public override UserOptionToggleArgs Deserialize(ref SpanReader reader)
    {
        var userOption = (UserOption)reader.ReadByte();

        return new UserOptionToggleArgs(userOption);
    }
}