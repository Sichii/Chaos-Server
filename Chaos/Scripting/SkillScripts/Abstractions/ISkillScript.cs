using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.SkillScripts.Abstractions;

public interface ISkillScript : IScript, IDeltaUpdatable
{
    bool CanUse(ActivationContext context);
    void OnForgotten(Aisling aisling);
    void OnLearned(Aisling aisling);
    void OnUse(ActivationContext context);
}