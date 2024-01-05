using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="MapInfoArgs" /> into a buffer
/// </summary>
public sealed class MapInfoConverter : PacketConverterBase<MapInfoArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.MapInfo;

    /// <inheritdoc />
    public override MapInfoArgs Deserialize(ref SpanReader reader)
    {
        var mapId = reader.ReadInt16();
        var width = reader.ReadByte();
        var height = reader.ReadByte();
        var flags = reader.ReadByte();
        _ = reader.ReadBytes(2); //LI: what is this for?
        var checkSum = reader.ReadUInt16();
        var name = reader.ReadString8();

        return new MapInfoArgs
        {
            MapId = mapId,
            Width = width,
            Height = height,
            Flags = flags,
            CheckSum = checkSum,
            Name = name
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MapInfoArgs args)
    {
        writer.WriteInt16(args.MapId);
        writer.WriteByte(args.Width);
        writer.WriteByte(args.Height);
        writer.WriteByte(args.Flags);
        writer.WriteBytes(new byte[2]); //LI: what is this for?
        writer.WriteUInt16(args.CheckSum);
        writer.WriteString8(args.Name);
    }
}