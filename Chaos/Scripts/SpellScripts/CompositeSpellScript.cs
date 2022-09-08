using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
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

    public void OnUse(ActivationContext context)
    {
        foreach (var component in Components)
            component.OnUse(context);
    }
}