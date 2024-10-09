using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="MapChangePendingArgs" />
/// </summary>
public class MapChangePendingConverter : PacketConverterBase<MapChangePendingArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.MapChangePending;

    /// <inheritdoc />
    public override MapChangePendingArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapChangePendingArgs args)
        => writer.WriteBytes(
            3,
            0,
            0,
            0,
            0,
            0);
}