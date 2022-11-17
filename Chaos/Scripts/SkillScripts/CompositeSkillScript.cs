using System.Runtime.InteropServices;
using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
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
    public void OnUse(SkillContext context)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUse(context);
    }
}