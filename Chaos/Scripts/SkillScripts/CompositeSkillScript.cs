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
    public void OnUse(SkillContext context)
    {
        foreach (var component in Components)
            component.OnUse(context);
    }
}