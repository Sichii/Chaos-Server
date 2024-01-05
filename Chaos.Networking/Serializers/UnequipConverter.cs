using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="UnequipArgs" /> into a buffer
/// </summary>
public sealed class UnequipConverter : PacketConverterBase<UnequipArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Unequip;

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