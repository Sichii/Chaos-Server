using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SwapSlotArgs" />
/// </summary>
public sealed record SwapSlotDeserializer : ClientPacketDeserializer<SwapSlotArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.SwapSlot;

    /// <inheritdoc />
    public override SwapSlotArgs Deserialize(ref SpanReader reader)
    {
        var panelType = (PanelType)reader.ReadByte();
        var slot1 = reader.ReadByte();
        var slot2 = reader.ReadByte();

        return new SwapSlotArgs(panelType, slot1, slot2);
    }
}