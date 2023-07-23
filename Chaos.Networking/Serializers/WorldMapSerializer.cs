using Chaos.Extensions.Networking;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="WorldMapArgs" /> into a buffer
/// </summary>
public sealed record WorldMapSerializer : ServerPacketSerializer<WorldMapArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.WorldMap;

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