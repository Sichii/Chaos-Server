using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Networking.Extensions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record WorldMapSerializer : ServerPacketSerializer<WorldMapArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.WorldMap;

    public override void Serialize(ref SpanWriter writer, WorldMapArgs args)
    {
        writer.WriteString8(args.FieldName);
        writer.WriteByte((byte)args.Nodes.Count);
        writer.WriteByte(args.ImageIndex);

        foreach (var node in args.Nodes)
        {
            writer.WritePoint16(node.Position);
            writer.WriteString8(node.Text);
            writer.WriteUInt16(node.CheckSum);
            writer.WriteUInt16(node.DestinationMapId);
            writer.WritePoint16(node.DestinationPoint);
        }
    }
}