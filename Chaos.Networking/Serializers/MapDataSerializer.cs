using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record MapDataSerializer : ServerPacketSerializer<MapDataArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.MapData;

    public override void Serialize(ref SpanWriter writer, MapDataArgs args)
    {
        writer.WriteUInt16(args.CurrentYIndex);
        writer.WriteData(args.MapData);
    }
}