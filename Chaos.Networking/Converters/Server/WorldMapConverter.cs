using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="WorldMapArgs" />
/// </summary>
public sealed class WorldMapConverter : PacketConverterBase<WorldMapArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.WorldMap;

    /// <inheritdoc />
    public override WorldMapArgs Deserialize(ref SpanReader reader)
    {
        var fieldName = reader.ReadString8();
        var nodeCount = reader.ReadByte();
        var fieldIndex = reader.ReadByte();

        var nodes = new List<WorldMapNodeInfo>(nodeCount);

        for (var i = 0; i < nodeCount; i++)
        {
            var screenPosition = reader.ReadPoint16();
            var text = reader.ReadString8();
            var checkSum = reader.ReadUInt16();
            var mapId = reader.ReadUInt16();
            var destinationPoint = reader.ReadPoint16();

            nodes.Add(
                new WorldMapNodeInfo
                {
                    ScreenPosition = (Point)screenPosition,
                    Text = text,
                    CheckSum = checkSum,
                    MapId = mapId,
                    DestinationPoint = destinationPoint
                });
        }

        return new WorldMapArgs
        {
            FieldName = fieldName,
            Nodes = nodes,
            FieldIndex = fieldIndex
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, WorldMapArgs args)
    {
        writer.WriteString8(args.FieldName);
        writer.WriteByte((byte)args.Nodes.Count);
        writer.WriteByte(args.FieldIndex);

        foreach (var node in args.Nodes)
        {
            writer.WritePoint16(node.ScreenPosition);
            writer.WriteString8(node.Text);
            writer.WriteUInt16(node.CheckSum);
            writer.WriteUInt16(node.MapId);
            writer.WritePoint16(node.DestinationPoint);
        }
    }
}