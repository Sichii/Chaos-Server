using Chaos.PanelObjects;

namespace Chaos.Scripts.Abstractions;

public abstract class SkillScriptBase : ScriptBase<Skill>
{
    protected SkillScriptBase(Skill skill)
        : base(skill) { }
}