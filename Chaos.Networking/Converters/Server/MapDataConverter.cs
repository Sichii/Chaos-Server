using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="MapDataArgs" />
/// </summary>
public sealed class MapDataConverter : PacketConverterBase<MapDataArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.MapData;

    /// <inheritdoc />
    public override MapDataArgs Deserialize(ref SpanReader reader)
    {
        var yIndex = reader.ReadUInt16();
        var mapData = reader.ReadData();

        return new MapDataArgs
        {
            CurrentYIndex = (byte)yIndex,
            MapData = mapData
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapDataArgs args)
    {
        writer.WriteUInt16(args.CurrentYIndex);
        writer.WriteData(args.MapData);
    }
}