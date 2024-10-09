using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="RefreshRequestArgs" />
/// </summary>
public sealed class RefreshRequestConverter : PacketConverterBase<RefreshRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.RefreshRequest;

    /// <inheritdoc />
    public override RefreshRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RefreshRequestArgs args) { }
}