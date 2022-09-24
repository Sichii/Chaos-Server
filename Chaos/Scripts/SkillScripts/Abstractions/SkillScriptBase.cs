using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class SkillScriptBase : ScriptBase, ISkillScript
{
    protected Skill Subject { get; }

    protected SkillScriptBase(Skill subject) => Subject = subject;
    public virtual void OnForgotten(Aisling aisling) { }
    public virtual void OnLearned(Aisling aisling) { }

    public virtual void OnUse(Creature source) { }
}