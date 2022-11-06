using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public abstract class ConfigurableSpellScriptBase : ConfigurableScriptBase<Spell>, ISpellScript
{
    /// <inheritdoc />
    protected ConfigurableSpellScriptBase(Spell subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(SpellContext context) { }
}