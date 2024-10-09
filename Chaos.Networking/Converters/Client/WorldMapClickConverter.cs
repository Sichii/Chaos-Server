using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="WorldMapClickArgs" />
/// </summary>
public sealed class WorldMapClickConverter : PacketConverterBase<WorldMapClickArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.WorldMapClick;

    /// <inheritdoc />
    public override WorldMapClickArgs Deserialize(ref SpanReader reader)
    {
        var checkSum = reader.ReadUInt16();
        var mapId = reader.ReadUInt16();
        var point = reader.ReadPoint16();

        return new WorldMapClickArgs
        {
            CheckSum = checkSum,
            MapId = mapId,
            Point = (Point)point
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, WorldMapClickArgs args)
    {
        writer.WriteUInt16(args.CheckSum);
        writer.WriteUInt16(args.MapId);
        writer.WritePoint16(args.Point);
    }
}