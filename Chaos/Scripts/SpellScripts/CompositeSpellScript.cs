using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
    protected Spell Subject { get; }
    public CompositeSpellScript(Spell subject) => Subject = subject;

    public void OnForgotten(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnForgotten(aisling);
    }

    public void OnLearned(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnLearned(aisling);
    }

    public void OnUse(SpellContext context)
    {
        foreach (var component in Components)
            component.OnUse(context);
    }
}