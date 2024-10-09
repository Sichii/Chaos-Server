using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SelfProfileRequestArgs" />
/// </summary>
public sealed class SelfProfileRequestConverter : PacketConverterBase<SelfProfileRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SelfProfileRequest;

    /// <inheritdoc />
    public override SelfProfileRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SelfProfileRequestArgs args) { }
}