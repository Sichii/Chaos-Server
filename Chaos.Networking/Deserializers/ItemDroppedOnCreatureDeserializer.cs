using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ItemDroppedOnCreatureArgs" />
/// </summary>
public sealed record ItemDroppedOnCreatureDeserializer : ClientPacketDeserializer<ItemDroppedOnCreatureArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemDroppedOnCreature;

    /// <inheritdoc />
    public override ItemDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var targetId = reader.ReadUInt32();
        var count = reader.ReadByte();

        return new ItemDroppedOnCreatureArgs(sourceSlot, targetId, count);
    }
}