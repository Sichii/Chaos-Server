using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="MapDataArgs" /> into a buffer
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