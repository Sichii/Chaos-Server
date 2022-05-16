using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record PickupDeserializer : ClientPacketDeserializer<PickupArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Pickup;

    public override PickupArgs Deserialize(ref SpanReader reader)
    {
        var destinationSlot = reader.ReadByte();
        var sourcePoint = reader.ReadPoint16();

        return new PickupArgs(destinationSlot, sourcePoint);
    }
}