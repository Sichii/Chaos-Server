using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="TurnArgs" />
/// </summary>
public sealed class TurnConverter : PacketConverterBase<TurnArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Turn;

    /// <inheritdoc />
    public override TurnArgs Deserialize(ref SpanReader reader)
    {
        var direction = reader.ReadByte();

        return new TurnArgs
        {
            Direction = (Direction)direction
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, TurnArgs args) => writer.WriteByte((byte)args.Direction);
}