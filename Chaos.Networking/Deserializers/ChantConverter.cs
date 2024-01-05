using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="DisplayChantArgs" />
/// </summary>
public sealed class ChantConverter : PacketConverterBase<DisplayChantArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Chant;

    /// <inheritdoc />
    public override DisplayChantArgs Deserialize(ref SpanReader reader)
    {
        var chantMessage = reader.ReadString8();

        return new DisplayChantArgs
        {
            ChantMessage = chantMessage
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayChantArgs args) => writer.WriteString8(args.ChantMessage);
}