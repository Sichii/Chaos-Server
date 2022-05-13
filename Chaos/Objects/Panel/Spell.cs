using Chaos.Objects.Panel.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public class Spell : PanelObjectBase, IScriptedSpell
{
    public byte CastLines { get; init; }

    public ISpellScript Script { get; set; } = null!;
    public override SpellTemplate Template { get; }

    public Spell(SpellTemplate template)
        : base(template)
    {
        Template = template;
        CastLines = template.BaseCastLines;
    }
}