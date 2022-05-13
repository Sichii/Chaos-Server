using Chaos.Objects;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.SpellScripts;

public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
    public void OnUse(ActivationContext context)
    {
        foreach (var component in Components)
            component.OnUse(context);
    }
}