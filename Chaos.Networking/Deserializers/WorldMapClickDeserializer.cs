using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Deserializers;

public record WorldMapClickDeserializer : ClientPacketDeserializer<WorldMapClickArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.WorldMapClick;

    public override WorldMapClickArgs Deserialize(ref SpanReader reader)
    {
        var nodeCheckSum = reader.ReadUInt16();
        var mapId = reader.ReadUInt16();
        var point = reader.ReadPoint16();

        return new WorldMapClickArgs(nodeCheckSum, mapId, point);
    }
}