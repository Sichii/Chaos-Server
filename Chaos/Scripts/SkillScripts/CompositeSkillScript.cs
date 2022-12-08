using System.Runtime.InteropServices;
using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public bool CanUse(SkillContext context)
    {
        var canUse = true;

        //if any component can't be used, the skill can't be used
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            canUse &= component.CanUse(context);

        return canUse;
    }

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
    public void OnUse(SkillContext context)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUse(context);
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}