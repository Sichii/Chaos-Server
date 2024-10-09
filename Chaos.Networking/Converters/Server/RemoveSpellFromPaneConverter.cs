using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="RemoveSpellFromPaneArgs" />
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