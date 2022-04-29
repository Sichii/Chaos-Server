using Chaos.Core.Definitions;
using Chaos.PanelObjects.Abstractions;
using Chaos.Templates;

namespace Chaos.PanelObjects;

/// <summary>
///     Represents an object that exists within the skill panel.
/// </summary>
public class Skill : PanelObjectBase
{
    public override SkillTemplate Template { get; }

    public Skill(SkillTemplate template)
        : base(template) => Template = template;

    public override bool CanUse(double cooldownReduction) =>
        (Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_SKILL_COOLDOWN_MS) && base.CanUse(cooldownReduction);
}