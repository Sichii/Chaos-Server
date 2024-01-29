using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Panel;

namespace Chaos.Collections;

public sealed class SpellBook : PanelBase<Spell>
{
    public SpellBook(IEnumerable<Spell>? spells = null)
        : base(
            PanelType.SpellBook,
            90,
            [
                0,
                36,
                72
            ])
    {
        spells ??= Array.Empty<Spell>();

        foreach (var spell in spells)
            Objects[spell.Slot] = spell;
    }
}