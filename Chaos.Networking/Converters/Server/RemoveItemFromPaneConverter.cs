using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="RemoveItemFromPaneArgs" />
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