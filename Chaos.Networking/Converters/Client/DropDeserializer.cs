using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="ItemDropArgs" />
/// </summary>
public sealed class ItemDropConverter : PacketConverterBase<ItemDropArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ItemDrop;

    /// <inheritdoc />
    public override ItemDropArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var destinationPoint = reader.ReadPoint16();
        var count = reader.ReadInt32();

        return new ItemDropArgs
        {
            SourceSlot = sourceSlot,
            DestinationPoint = (Point)destinationPoint,
            Count = count
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ItemDropArgs args)
    {
        writer.WriteByte(args.SourceSlot);
        writer.WritePoint16(args.DestinationPoint);
        writer.WriteInt32(args.Count);
    }
}