using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public interface ISkillScript : IScript
{
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(SkillContext context);
}