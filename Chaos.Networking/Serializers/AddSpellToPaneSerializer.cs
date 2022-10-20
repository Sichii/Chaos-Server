using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record AddSpellToPaneSerializer : ServerPacketSerializer<AddSpellToPaneArgs>
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