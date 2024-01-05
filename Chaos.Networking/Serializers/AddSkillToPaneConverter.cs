using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="AddSkillToPaneArgs" /> into a buffer
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