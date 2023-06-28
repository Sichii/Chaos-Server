using System.Runtime.InteropServices;
using Chaos.Models.Data;
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
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanUse(context))
                return false;

        return true;
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