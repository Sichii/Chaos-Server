using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="RemoveSpellFromPaneArgs" /> into a buffer
/// </summary>
public sealed class RemoveSpellFromPaneConverter : PacketConverterBase<RemoveSpellFromPaneArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RemoveSpellFromPane;

    /// <inheritdoc />
    public override RemoveSpellFromPaneArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();

        return new RemoveSpellFromPaneArgs
        {
            Slot = slot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveSpellFromPaneArgs args) => writer.WriteByte(args.Slot);
}