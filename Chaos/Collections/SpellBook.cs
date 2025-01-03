#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a panel of spells
/// </summary>
public sealed class SpellBook : KnowledgeBookBase<Spell>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpellBook" /> class.
    /// </summary>
    /// <param name="spells">
    ///     The spells to populate the panel with
    /// </param>
    public SpellBook(IEnumerable<Spell>? spells = null)
        : base(PanelType.SpellBook, spells) { }
}