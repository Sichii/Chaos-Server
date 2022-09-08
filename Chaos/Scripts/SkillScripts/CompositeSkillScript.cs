using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
{
    public void OnForgotten(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnForgotten(aisling);
    }

    public void OnLearned(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnLearned(aisling);
    }

    public void OnUse(Creature source)
    {
        foreach (var component in Components)
            component.OnUse(source);
    }
}