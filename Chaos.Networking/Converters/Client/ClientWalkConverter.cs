using Chaos.Geometry.Abstractions.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ClientWalkArgs" />
/// </summary>
public sealed class ClientWalkConverter : PacketConverterBase<ClientWalkArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ClientWalk;

    /// <inheritdoc />
    public override ClientWalkArgs Deserialize(ref SpanReader reader)
    {
        var direction = reader.ReadByte();
        var stepCount = reader.ReadByte();

        return new ClientWalkArgs
        {
            Direction = (Direction)direction,
            StepCount = stepCount
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ClientWalkArgs args)
    {
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(args.StepCount);
    }
}