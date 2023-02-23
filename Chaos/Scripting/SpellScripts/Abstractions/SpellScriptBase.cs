using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.SpellScripts.Abstractions;

public abstract class SpellScriptBase : SubjectiveScriptBase<Spell>, ISpellScript
{
    /// <inheritdoc />
    protected SpellScriptBase(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanUse(SpellContext context) => context.Source.IsAlive && context.Target.IsAlive;

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(SpellContext context) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}