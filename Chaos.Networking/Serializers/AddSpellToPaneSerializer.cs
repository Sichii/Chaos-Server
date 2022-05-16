using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record AddSpellToPaneSerializer : ServerPacketSerializer<AddSpellToPaneArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.AddSpellToPane;

    public override void Serialize(ref SpanWriter writer, AddSpellToPaneArgs args)
    {
        writer.WriteByte(args.Spell.Slot);
        writer.WriteUInt16(args.Spell.Sprite);
        writer.WriteByte((byte)args.Spell.SpellType);
        writer.WriteString8(args.Spell.Name);
        writer.WriteString8(args.Spell.Prompt);
        writer.WriteByte(args.Spell.CastLines);
    }
}