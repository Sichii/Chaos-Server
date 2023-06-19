using System.Runtime.InteropServices;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeSpellScript : CompositeScriptBase<ISpellScript>, ISpellScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public bool CanUse(SpellContext context)
    {
        var canUse = true;

        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            canUse &= script.CanUse(context);

        return canUse;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnForgotten(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnForgotten(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnLearned(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnLearned(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnUse(SpellContext context)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnUse(context);
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}