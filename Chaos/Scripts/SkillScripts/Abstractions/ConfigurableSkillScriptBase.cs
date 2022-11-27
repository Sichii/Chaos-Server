using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class ConfigurableSkillScriptBase : ConfigurableScriptBase<Skill>, ISkillScript
{
    /// <inheritdoc />
    protected ConfigurableSkillScriptBase(Skill subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanUse(SkillContext context) => context.Source.IsAlive;

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(SkillContext context) { }
}