using System.Runtime.InteropServices;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public bool CanUse(ActivationContext context)
    {
        var canUse = true;

        //if any script can't be used, the skill can't be used
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
    public void OnUse(ActivationContext context)
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