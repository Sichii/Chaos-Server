using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record PickupDeserializer : ClientPacketDeserializer<PickupArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Pickup;

    public override PickupArgs Deserialize(ref SpanReader reader)
    {
        var destinationSlot = reader.ReadByte();
        Point sourcePoint = reader.ReadPoint16();

        return new PickupArgs(destinationSlot, sourcePoint);
    }
}