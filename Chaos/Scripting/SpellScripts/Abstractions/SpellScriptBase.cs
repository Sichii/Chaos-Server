using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.SpellScripts.Abstractions;

public abstract class SpellScriptBase : SubjectiveScriptBase<Spell>, ISpellScript
{
    /// <inheritdoc />
    protected SpellScriptBase(Spell subject)
        : base(subject) { }

    /// <inheritdoc />
    public virtual bool CanUse(SpellContext context) => context.Source.IsAlive && (context.TargetCreature?.IsAlive ?? true);

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(SpellContext context) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}