using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="UserOptionToggleArgs" />
/// </summary>
public sealed class UserOptionToggleConverter : PacketConverterBase<UserOptionToggleArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.UserOptionToggle;

    /// <inheritdoc />
    public override UserOptionToggleArgs Deserialize(ref SpanReader reader)
    {
        var userOption = reader.ReadByte();

        return new UserOptionToggleArgs
        {
            UserOption = (UserOption)userOption
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, UserOptionToggleArgs args) => writer.WriteByte((byte)args.UserOption);
}