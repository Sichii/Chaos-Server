using Chaos.Containers.Abstractions;
using Chaos.Networking.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Serialization.Interfaces;

namespace Chaos.Containers;

public class SpellBook : PanelBase<Spell>
{
    public SpellBook()
        : base(
            PanelType.SpellBook,
            90,
            new byte[] { 0, 36, 72 }) { }

    public SpellBook(IEnumerable<SerializableSpell> serializedSpells, ISerialTransformService<Spell, SerializableSpell> spellTransformer)
        : this()
    {
        foreach (var serialized in serializedSpells)
        {
            var spell = spellTransformer.Transform(serialized);
            Objects[spell.Slot] = spell;
        }
    }
}