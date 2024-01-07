using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="ItemDroppedOnCreatureArgs" />
/// </summary>
public sealed class ItemDroppedOnCreatureConverter : PacketConverterBase<ItemDroppedOnCreatureArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ItemDroppedOnCreature;

    /// <inheritdoc />
    public override ItemDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var targetId = reader.ReadUInt32();
        var count = reader.ReadByte();

        return new ItemDroppedOnCreatureArgs
        {
            SourceSlot = sourceSlot,
            TargetId = targetId,
            Count = count
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ItemDroppedOnCreatureArgs args)
    {
        writer.WriteByte(args.SourceSlot);
        writer.WriteUInt32(args.TargetId);
        writer.WriteByte(args.Count);
    }
}