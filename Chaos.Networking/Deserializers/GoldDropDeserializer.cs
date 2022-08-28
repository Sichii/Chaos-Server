using Chaos.Entities.Networking.Client;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record GoldDropDeserializer : ClientPacketDeserializer<GoldDropArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDrop;

    public override GoldDropArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        Point destinationPoint = reader.ReadPoint16();

        return new GoldDropArgs(amount, destinationPoint);
    }
}