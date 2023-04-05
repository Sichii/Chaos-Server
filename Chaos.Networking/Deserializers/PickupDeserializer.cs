using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="PickupArgs" />
/// </summary>
public sealed record PickupDeserializer : ClientPacketDeserializer<PickupArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Pickup;

    /// <inheritdoc />
    public override PickupArgs Deserialize(ref SpanReader reader)
    {
        var destinationSlot = reader.ReadByte();
        Point sourcePoint = reader.ReadPoint16();

        return new PickupArgs(destinationSlot, sourcePoint);
    }
}