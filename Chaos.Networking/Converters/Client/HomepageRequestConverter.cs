using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="HomepageRequestArgs" />
/// </summary>
public sealed class HomepageRequestConverter : PacketConverterBase<HomepageRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.HomepageRequest;

    /// <inheritdoc />
    public override HomepageRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, HomepageRequestArgs args) { }
}