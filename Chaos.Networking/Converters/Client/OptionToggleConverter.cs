using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="OptionToggleArgs" />
/// </summary>
public sealed class OptionToggleConverter : PacketConverterBase<OptionToggleArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.OptionToggle;

    /// <inheritdoc />
    public override OptionToggleArgs Deserialize(ref SpanReader reader)
    {
        var userOption = reader.ReadByte();

        return new OptionToggleArgs
        {
            UserOption = (UserOption)userOption
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, OptionToggleArgs args) => writer.WriteByte((byte)args.UserOption);
}