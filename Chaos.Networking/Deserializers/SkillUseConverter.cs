using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SkillUseArgs" />
/// </summary>
public sealed class SkillUseConverter : PacketConverterBase<SkillUseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.UseSkill;

    /// <inheritdoc />
    public override SkillUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new SkillUseArgs
        {
            SourceSlot = sourceSlot
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SkillUseArgs args) => writer.WriteByte(args.SourceSlot);
}