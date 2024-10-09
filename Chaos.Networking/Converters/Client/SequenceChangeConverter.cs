using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SequenceChangeArgs" />
/// </summary>
public sealed class SequenceChangeConverter : PacketConverterBase<SequenceChangeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SequenceChange;

    /// <inheritdoc />
    /// This converter is specially handled, and will have it's sequence populated by the serializer
    public override SequenceChangeArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SequenceChangeArgs args) { }
}