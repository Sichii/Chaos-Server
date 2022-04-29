using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Deserializers;

public record ItemDroppedOnCreatureDeserializer : ClientPacketDeserializer<ItemDroppedOnCreatureArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemDroppedOnCreature;

    public override ItemDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var targetId = reader.ReadUInt32();
        var count = reader.ReadByte();

        return new ItemDroppedOnCreatureArgs(sourceSlot, targetId, count);
    }
}