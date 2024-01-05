using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="RemoveItemFromPaneArgs" /> into a buffer
/// </summary>
public sealed class RemoveItemFromPaneConverter : PacketConverterBase<RemoveItemFromPaneArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RemoveItemFromPane;

    /// <inheritdoc />
    public override RemoveItemFromPaneArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();

        return new RemoveItemFromPaneArgs
        {
            Slot = slot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveItemFromPaneArgs args) => writer.WriteByte(args.Slot);
}