using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayUnequipArgs" />
/// </summary>
public sealed class DisplayUnequipConverter : PacketConverterBase<DisplayUnequipArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayUnequip;

    /// <inheritdoc />
    public override DisplayUnequipArgs Deserialize(ref SpanReader reader)
    {
        var equipmentSlot = reader.ReadByte();

        return new DisplayUnequipArgs
        {
            EquipmentSlot = (EquipmentSlot)equipmentSlot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayUnequipArgs args) => writer.WriteByte((byte)args.EquipmentSlot);
}