using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="UserIdArgs" /> into a buffer
/// </summary>
public sealed class UserIdConverter : PacketConverterBase<UserIdArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.UserId;

    /// <inheritdoc />
    public override UserIdArgs Deserialize(ref SpanReader reader)
    {
        var id = reader.ReadUInt32();
        var direction = reader.ReadByte();
        _ = reader.ReadByte(); //LI: what is this for?
        var baseClass = reader.ReadByte();
        _ = reader.ReadByte(); //LI: what is this for?
        _ = reader.ReadByte(); //LI: what is this for?
        _ = reader.ReadByte(); //LI: what is this for?

        return new UserIdArgs
        {
            Id = id,
            Direction = (Direction)direction,
            BaseClass = (BaseClass)baseClass
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, UserIdArgs args)
    {
        writer.WriteUInt32(args.Id);
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(0); //LI: what is this for?
        writer.WriteByte((byte)args.BaseClass);
        writer.WriteByte(0); //LI: what is this for?
        writer.WriteByte(0); //LI: what is this for?
        writer.WriteByte(0); //LI: what is this for?
    }
}