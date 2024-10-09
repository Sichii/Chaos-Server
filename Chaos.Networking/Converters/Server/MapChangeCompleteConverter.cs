using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="MapChangeCompleteArgs" />
/// </summary>
public class MapChangeCompleteConverter : PacketConverterBase<MapChangeCompleteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.MapChangeComplete;

    /// <inheritdoc />
    public override MapChangeCompleteArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapChangeCompleteArgs args) => writer.WriteBytes(0, 0);
}