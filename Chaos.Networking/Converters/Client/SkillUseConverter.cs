using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SkillUseArgs" />
/// </summary>
public sealed class SkillUseConverter : PacketConverterBase<SkillUseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SkillUse;

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