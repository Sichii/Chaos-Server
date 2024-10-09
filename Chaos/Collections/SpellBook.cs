using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;

namespace Chaos.Collections;

public sealed class SpellBook : KnowledgeBookBase<Spell>
{
    public SpellBook(IEnumerable<Spell>? spells = null)
        : base(PanelType.SpellBook, spells) { }
}