using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="CreatureWalkArgs" />
/// </summary>
public sealed class CreatureWalkConverter : PacketConverterBase<CreatureWalkArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.CreatureWalk;

    /// <inheritdoc />
    public override CreatureWalkArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();
        var oldPoint = reader.ReadPoint16();
        var direction = reader.ReadByte();

        //_ = reader.ReadByte(); //LI: what does this do?

        return new CreatureWalkArgs
        {
            SourceId = sourceId,
            OldPoint = (Point)oldPoint,
            Direction = (Direction)direction
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CreatureWalkArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WritePoint16(args.OldPoint);
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(0); //LI: what does this do?
    }
}