using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public interface ISkillScript : IScript, IDeltaUpdatable
{
    bool CanUse(ActivationContext context);
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(ActivationContext context);
}