using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class SkillScriptBase : SubjectiveScriptBase<Skill>, ISkillScript
{
    /// <inheritdoc />
    protected SkillScriptBase(Skill subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanUse(SkillContext context) => context.Source.IsAlive;

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(SkillContext context) { }
}