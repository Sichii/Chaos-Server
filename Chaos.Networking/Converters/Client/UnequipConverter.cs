using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="UnequipArgs" />
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