using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

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