using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="UnequipArgs" />
/// </summary>
public sealed record UnequipDeserializer : ClientPacketDeserializer<UnequipArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Unequip;

    /// <inheritdoc />
    public override UnequipArgs Deserialize(ref SpanReader reader)
    {
        var equipmentSlot = (EquipmentSlot)reader.ReadByte();

        return new UnequipArgs(equipmentSlot);
    }
}