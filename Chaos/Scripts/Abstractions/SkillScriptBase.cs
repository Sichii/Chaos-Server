using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Abstractions;

public abstract class SkillScriptBase : ScriptBase, ISkillScript
{
    protected Skill Skill { get; }

    protected SkillScriptBase(Skill skill) => Skill = skill;
    public virtual void OnForgotten(Aisling aisling) { }
    public virtual void OnLearned(Aisling aisling) { }

    public virtual void OnUse(Creature source) { }
}