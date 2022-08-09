using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record SwapSlotDeserializer : ClientPacketDeserializer<SwapSlotArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SwapSlot;

    public override SwapSlotArgs Deserialize(ref SpanReader reader)
    {
        var panelType = (PanelType)reader.ReadByte();
        var slot1 = reader.ReadByte();
        var slot2 = reader.ReadByte();

        return new SwapSlotArgs(panelType, slot1, slot2);
    }
}