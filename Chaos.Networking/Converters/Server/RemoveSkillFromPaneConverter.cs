using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="RemoveSkillFromPaneArgs" /> into a buffer
/// </summary>
public sealed class RemoveSkillFromPaneConverter : PacketConverterBase<RemoveSkillFromPaneArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RemoveSkillFromPane;

    /// <inheritdoc />
    public override RemoveSkillFromPaneArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();

        return new RemoveSkillFromPaneArgs
        {
            Slot = slot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveSkillFromPaneArgs args) => writer.WriteByte(args.Slot);
}