using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record GoldDropDeserializer : ClientPacketDeserializer<GoldDropArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDrop;

    public override GoldDropArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        Point destinationPoint = reader.ReadPoint16();

        return new GoldDropArgs(amount, destinationPoint);
    }
}