using Chaos.Objects.Panel;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class NoOpSpellScript : SpellScriptBase
{
    public NoOpSpellScript(Spell spell)
        : base(spell) { }
}