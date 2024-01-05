using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="ConfirmClientWalkArgs" /> into a buffer
/// </summary>
public sealed class ConfirmClientWalkConverter : PacketConverterBase<ConfirmClientWalkArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ConfirmClientWalk;

    /// <inheritdoc />
    public override ConfirmClientWalkArgs Deserialize(ref SpanReader reader)
    {
        var direction = reader.ReadByte();
        var oldPoint = reader.ReadPoint16();

        //these confirmed do nothing, but crash the client if not sent
        //_ = reader.ReadUInt16();
        //_ = reader.ReadUInt16();
        //_ = reader.ReadByte();

        return new ConfirmClientWalkArgs
        {
            Direction = (Direction)direction,
            OldPoint = (Point)oldPoint
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ConfirmClientWalkArgs args)
    {
        writer.WriteBytes((byte)args.Direction);
        writer.WritePoint16(args.OldPoint);

        //these confirmed do nothing, but crash the client if not sent
        writer.WriteUInt16(11);
        writer.WriteUInt16(11);
        writer.WriteByte(1);
    }
}