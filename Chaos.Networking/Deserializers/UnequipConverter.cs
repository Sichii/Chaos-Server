using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="UnequipArgs" />
/// </summary>
public sealed class UnequipConverter : PacketConverterBase<UnequipArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Unequip;

    /// <inheritdoc />
    public override UnequipArgs Deserialize(ref SpanReader reader)
    {
        var equipmentSlot = reader.ReadByte();

        return new UnequipArgs
        {
            EquipmentSlot = (EquipmentSlot)equipmentSlot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, UnequipArgs args) => writer.WriteByte((byte)args.EquipmentSlot);
}