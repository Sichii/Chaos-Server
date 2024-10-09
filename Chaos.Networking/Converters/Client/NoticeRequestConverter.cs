using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="NoticeRequestArgs" />
/// </summary>
public sealed class NoticeRequestConverter : PacketConverterBase<NoticeRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.NoticeRequest;

    /// <inheritdoc />
    public override NoticeRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, NoticeRequestArgs args) { }
}