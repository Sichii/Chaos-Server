using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="CreatureTurnArgs" /> into a buffer
/// </summary>
public sealed class CreatureTurnConverter : PacketConverterBase<CreatureTurnArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.CreatureTurn;

    /// <inheritdoc />
    public override CreatureTurnArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();
        var direction = reader.ReadByte();

        return new CreatureTurnArgs
        {
            SourceId = sourceId,
            Direction = (Direction)direction
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CreatureTurnArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte((byte)args.Direction);
    }
}