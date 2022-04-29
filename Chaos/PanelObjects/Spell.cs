using Chaos.Core.Definitions;
using Chaos.PanelObjects.Abstractions;
using Chaos.Templates;

namespace Chaos.PanelObjects;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public class Spell : PanelObjectBase
{
    public byte CastLines { get; init; }
    public override SpellTemplate Template { get; }

    public Spell(SpellTemplate template)
        : base(template)
    {
        Template = template;
        CastLines = template.BaseCastLines;
    }

    public override bool CanUse(double cooldownReduction) =>
        (Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_SPELL_COOLDOWN_MS) && base.CanUse(cooldownReduction);
}