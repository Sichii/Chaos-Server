using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="ItemUseArgs" />
/// </summary>
public sealed class ItemUseConverter : PacketConverterBase<ItemUseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.UseItem;

    /// <inheritdoc />
    public override ItemUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new ItemUseArgs
        {
            SourceSlot = sourceSlot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ItemUseArgs args) => writer.WriteByte(args.SourceSlot);
}