using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
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
    public void OnUse(Creature source)
    {
        foreach (var component in Components)
            component.OnUse(source);
    }
}