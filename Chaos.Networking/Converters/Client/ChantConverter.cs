using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ChantArgs" />
/// </summary>
public sealed class ChantConverter : PacketConverterBase<ChantArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Chant;

    /// <inheritdoc />
    public override ChantArgs Deserialize(ref SpanReader reader)
    {
        var chantMessage = reader.ReadString8();

        return new ChantArgs
        {
            ChantMessage = chantMessage
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ChantArgs args) => writer.WriteString8(args.ChantMessage);
}