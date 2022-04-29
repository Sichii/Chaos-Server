using Chaos.PanelObjects;

namespace Chaos.Scripts.Abstractions;

public abstract class SpellScriptBase : ScriptBase<Spell>
{
    protected SpellScriptBase(Spell spell)
        : base(spell) { }
}