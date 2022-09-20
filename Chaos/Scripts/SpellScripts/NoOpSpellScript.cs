using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class NoOpSpellScript : SpellScriptBase
{
    public NoOpSpellScript(Spell subject)
        : base(subject) { }
}