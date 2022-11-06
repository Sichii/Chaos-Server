using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
    /// <inheritdoc />
    public void OnForgotten(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnForgotten(aisling);
    }

    /// <inheritdoc />
    public void OnLearned(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnLearned(aisling);
    }

    /// <inheritdoc />
    public void OnUse(SpellContext context)
    {
        foreach (var component in Components)
            component.OnUse(context);
    }
}