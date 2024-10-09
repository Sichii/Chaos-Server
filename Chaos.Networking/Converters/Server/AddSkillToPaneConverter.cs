using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="AddSkillToPaneArgs" />
/// </summary>
public sealed class AddSkillToPaneConverter : PacketConverterBase<AddSkillToPaneArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.AddSkillToPane;

    /// <inheritdoc />
    public override AddSkillToPaneArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var sprite = reader.ReadUInt16();
        var panelName = reader.ReadString8();

        return new AddSkillToPaneArgs
        {
            Skill = new SkillInfo
            {
                Slot = slot,
                Sprite = sprite,
                PanelName = panelName
            }
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AddSkillToPaneArgs args)
    {
        writer.WriteByte(args.Skill.Slot);
        writer.WriteUInt16(args.Skill.Sprite);
        writer.WriteString8(args.Skill.PanelName);
    }
}