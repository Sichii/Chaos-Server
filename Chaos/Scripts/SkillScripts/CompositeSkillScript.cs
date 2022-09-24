using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class CompositeSkillScript : CompositeScriptBase<ISkillScript>, ISkillScript
{
    protected Skill Subject { get; }
    public CompositeSkillScript(Skill subject) => Subject = subject;

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