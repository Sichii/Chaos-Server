using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.SkillScripts;

public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
{
    public void OnUse(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnUse(aisling);
    }
}