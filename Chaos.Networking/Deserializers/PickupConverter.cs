using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="PickupArgs" />
/// </summary>
public sealed class PickupConverter : PacketConverterBase<PickupArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Pickup;

    /// <inheritdoc />
    public override PickupArgs Deserialize(ref SpanReader reader)
    {
        var destinationSlot = reader.ReadByte();
        var sourcePoint = reader.ReadPoint16();

        return new PickupArgs
        {
            DestinationSlot = destinationSlot,
            SourcePoint = (Point)sourcePoint
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, PickupArgs args)
    {
        writer.WriteByte(args.DestinationSlot);
        writer.WritePoint16(args.SourcePoint);
    }
}