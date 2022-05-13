using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public abstract class SkillScriptBase : ScriptBase, ISkillScript
{
    protected Skill Source { get; }

    protected SkillScriptBase(Skill skill) => Source = skill;

    public abstract void OnUse(User user);
}