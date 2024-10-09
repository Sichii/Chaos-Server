using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SwapSlotArgs" />
/// </summary>
public sealed class SwapSlotConverter : PacketConverterBase<SwapSlotArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SwapSlot;

    /// <inheritdoc />
    public override SwapSlotArgs Deserialize(ref SpanReader reader)
    {
        var panelType = reader.ReadByte();
        var slot1 = reader.ReadByte();
        var slot2 = reader.ReadByte();

        return new SwapSlotArgs
        {
            PanelType = (PanelType)panelType,
            Slot1 = slot1,
            Slot2 = slot2
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SwapSlotArgs args)
    {
        writer.WriteByte((byte)args.PanelType);
        writer.WriteByte(args.Slot1);
        writer.WriteByte(args.Slot2);
    }
}