using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.SpellScripts.Abstractions;

public abstract class ConfigurableSpellScriptBase : ConfigurableScriptBase<Spell>, ISpellScript
{
    /// <inheritdoc />
    protected ConfigurableSpellScriptBase(Spell subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanUse(SpellContext context) => true;

    /// <inheritdoc />
    public virtual void OnUse(SpellContext context) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}