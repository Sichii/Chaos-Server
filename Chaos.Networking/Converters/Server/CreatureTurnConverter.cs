using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="CreatureTurnArgs" />
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