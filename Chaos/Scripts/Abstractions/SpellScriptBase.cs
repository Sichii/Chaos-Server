using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public abstract class SpellScriptBase : ScriptBase, ISpellScript
{
    protected Spell Source { get; }

    protected SpellScriptBase(Spell spell) => Source = spell;

    public abstract void OnUse(ActivationContext context);
}