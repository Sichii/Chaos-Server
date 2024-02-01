using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="MapDataRequestArgs" />
/// </summary>
public sealed class MapDataRequestConverter : PacketConverterBase<MapDataRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.MapDataRequest;

    /// <inheritdoc />
    public override MapDataRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapDataRequestArgs args) { }
}