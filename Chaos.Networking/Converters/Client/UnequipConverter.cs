using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="UnequipRequestArgs" />
/// </summary>
public sealed class UnequipConverter : PacketConverterBase<UnequipRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Unequip;

    /// <inheritdoc />
    public override UnequipRequestArgs Deserialize(ref SpanReader reader)
    {
        var equipmentSlot = reader.ReadByte();

        return new UnequipRequestArgs
        {
            EquipmentSlot = (EquipmentSlot)equipmentSlot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, UnequipRequestArgs requestArgs)
        => writer.WriteByte((byte)requestArgs.EquipmentSlot);
}