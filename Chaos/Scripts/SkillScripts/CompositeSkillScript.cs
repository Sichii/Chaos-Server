using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;

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

    public void OnUse(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnUse(aisling);
    }
}