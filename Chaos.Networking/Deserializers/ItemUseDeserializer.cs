using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record ItemUseDeserializer : ClientPacketDeserializer<ItemUseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemUse;

    public override ItemUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new ItemUseArgs(sourceSlot);
    }
}