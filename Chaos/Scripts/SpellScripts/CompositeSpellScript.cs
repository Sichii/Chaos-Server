using System.Runtime.InteropServices;
using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnForgotten(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnForgotten(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnLearned(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnLearned(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnUse(SpellContext context)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUse(context);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public bool CanUse(SpellContext context)
    {
        var canUse = true;

        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            canUse &= component.CanUse(context);

        return canUse;
    }
}