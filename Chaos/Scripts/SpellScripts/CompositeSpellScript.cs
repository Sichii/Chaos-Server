using System.Runtime.InteropServices;
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
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnForgotten(aisling);
    }

    /// <inheritdoc />
    public void OnLearned(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnLearned(aisling);
    }

    /// <inheritdoc />
    public void OnUse(SpellContext context)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUse(context);
    }
}