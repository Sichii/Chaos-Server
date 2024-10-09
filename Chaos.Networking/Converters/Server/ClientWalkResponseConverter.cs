using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="ClientWalkResponseArgs" />
/// </summary>
public sealed class ClientWalkResponseConverter : PacketConverterBase<ClientWalkResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ClientWalkResponse;

    /// <inheritdoc />
    public override ClientWalkResponseArgs Deserialize(ref SpanReader reader)
    {
        var direction = reader.ReadByte();
        var oldPoint = reader.ReadPoint16();

        //these confirmed do nothing, but crash the client if not sent
        //_ = reader.ReadUInt16();
        //_ = reader.ReadUInt16();
        //_ = reader.ReadByte();

        return new ClientWalkResponseArgs
        {
            Direction = (Direction)direction,
            OldPoint = (Point)oldPoint
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ClientWalkResponseArgs args)
    {
        writer.WriteBytes((byte)args.Direction);
        writer.WritePoint16(args.OldPoint);

        //these confirmed do nothing, but crash the client if not sent
        writer.WriteUInt16(11);
        writer.WriteUInt16(11);
        writer.WriteByte(1);
    }
}