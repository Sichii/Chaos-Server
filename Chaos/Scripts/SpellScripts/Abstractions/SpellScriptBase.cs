using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public abstract class SpellScriptBase : ScriptBase, ISpellScript
{
    protected Spell Subject { get; }

    protected SpellScriptBase(Spell subject) => Subject = subject;
    public virtual void OnForgotten(Aisling aisling) { }
    public virtual void OnLearned(Aisling aisling) { }

    public virtual void OnUse(SpellContext context) { }
}