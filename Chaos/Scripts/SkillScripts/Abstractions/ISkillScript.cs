using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public interface ISkillScript : IScript, IDeltaUpdatable
{
    bool CanUse(SkillContext context);
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(SkillContext context);
}