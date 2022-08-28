using Chaos.Entities.Networking.Client;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record WorldMapClickDeserializer : ClientPacketDeserializer<WorldMapClickArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.WorldMapClick;

    public override WorldMapClickArgs Deserialize(ref SpanReader reader)
    {
        var nodeCheckSum = reader.ReadUInt16();
        var mapId = reader.ReadUInt16();
        Point point = reader.ReadPoint16();

        return new WorldMapClickArgs(nodeCheckSum, mapId, point);
    }
}