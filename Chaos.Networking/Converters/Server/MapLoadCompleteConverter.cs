using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="MapLoadCompleteArgs" />
/// </summary>
public class MapLoadCompleteConverter : PacketConverterBase<MapLoadCompleteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.MapLoadComplete;

    /// <inheritdoc />
    public override MapLoadCompleteArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapLoadCompleteArgs args) => writer.WriteByte(0);
}