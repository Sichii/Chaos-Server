using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record ItemDropDeserializer : ClientPacketDeserializer<ItemDropArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemDrop;

    public override ItemDropArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var destinationPoint = reader.ReadPoint16();
        var count = reader.ReadInt32();

        return new ItemDropArgs(sourceSlot, destinationPoint, count);
    }
}