using Chaos.Objects.Panel.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the skill panel.
/// </summary>
public class Skill : PanelObjectBase, IScriptedSkill
{
    public ISkillScript Script { get; set; } = null!;
    public override SkillTemplate Template { get; }

    public Skill(SkillTemplate template)
        : base(template) => Template = template;
}