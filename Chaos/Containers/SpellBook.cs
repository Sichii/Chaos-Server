using Chaos.Common.Definitions;
using Chaos.Containers.Abstractions;
using Chaos.Objects.Panel;

namespace Chaos.Containers;

public class SpellBook : PanelBase<Spell>
{
    public SpellBook(IEnumerable<Spell>? spells = null)
        : base(
            PanelType.SpellBook,
            90,
            new byte[] { 0, 36, 72 })
    {
        spells ??= Array.Empty<Spell>();

        foreach (var spell in spells)
            Objects[spell.Slot] = spell;
    }
}