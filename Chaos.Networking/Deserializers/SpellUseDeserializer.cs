using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record SpellUseDeserializer : ClientPacketDeserializer<SpellUseArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SpellUse;

    public override SpellUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var argsData = reader.ReadData();

        return new SpellUseArgs(sourceSlot, argsData);
    }
}