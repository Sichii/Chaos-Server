using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class NoOpScript : SpellScriptBase
{
    public NoOpScript(Spell subject)
        : base(subject) { }
}