using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record ItemDropDeserializer : ClientPacketDeserializer<ItemDropArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemDrop;

    public override ItemDropArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        Point destinationPoint = reader.ReadPoint16();
        var count = reader.ReadInt32();

        return new ItemDropArgs(sourceSlot, destinationPoint, count);
    }
}