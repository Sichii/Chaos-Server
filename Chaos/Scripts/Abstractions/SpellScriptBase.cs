using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;

namespace Chaos.Scripts.Abstractions;

public abstract class SpellScriptBase : ScriptBase, ISpellScript
{
    protected Spell Source { get; }

    protected SpellScriptBase(Spell spell) => Source = spell;
    public virtual void OnForgotten(Aisling aisling) { }
    public virtual void OnLearned(Aisling aisling) { }

    public virtual void OnUse(ActivationContext context) { }
}