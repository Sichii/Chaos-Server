using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="AddSpellToPaneArgs" />
/// </summary>
public sealed class AddSpellToPaneConverter : PacketConverterBase<AddSpellToPaneArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.AddSpellToPane;

    /// <inheritdoc />
    public override AddSpellToPaneArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var sprite = reader.ReadUInt16();
        var spellType = reader.ReadByte();
        var panelName = reader.ReadString8();
        var prompt = reader.ReadString8();
        var castLines = reader.ReadByte();

        return new AddSpellToPaneArgs
        {
            Spell = new SpellInfo
            {
                Slot = slot,
                Sprite = sprite,
                SpellType = (SpellType)spellType,
                PanelName = panelName,
                Prompt = prompt,
                CastLines = castLines
            }
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AddSpellToPaneArgs args)
    {
        writer.WriteByte(args.Spell.Slot);
        writer.WriteUInt16(args.Spell.Sprite);
        writer.WriteByte((byte)args.Spell.SpellType);
        writer.WriteString8(args.Spell.PanelName);
        writer.WriteString8(args.Spell.Prompt);
        writer.WriteByte(args.Spell.CastLines);
    }
}