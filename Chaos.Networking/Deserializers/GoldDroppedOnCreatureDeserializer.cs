using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record GoldDroppedOnCreatureDeserializer : ClientPacketDeserializer<GoldDroppedOnCreatureArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDroppedOnCreature;

    public override GoldDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        var targetId = reader.ReadUInt32();

        return new GoldDroppedOnCreatureArgs(amount, targetId);
    }
}