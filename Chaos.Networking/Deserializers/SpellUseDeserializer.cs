using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

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