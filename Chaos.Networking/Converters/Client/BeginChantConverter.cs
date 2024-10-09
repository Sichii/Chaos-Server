using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="BeginChantArgs" />
/// </summary>
public sealed class BeginChantConverter : PacketConverterBase<BeginChantArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.BeginChant;

    /// <inheritdoc />
    public override BeginChantArgs Deserialize(ref SpanReader reader)
    {
        var castLineCount = reader.ReadByte();

        return new BeginChantArgs
        {
            CastLineCount = castLineCount
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, BeginChantArgs args) => writer.WriteByte(args.CastLineCount);
}