using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record SpellUseDeserializer : ClientPacketDeserializer<SpellUseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SpellUse;

    public override SpellUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var argsData = reader.ReadData();

        return new SpellUseArgs(sourceSlot, argsData);
    }
}