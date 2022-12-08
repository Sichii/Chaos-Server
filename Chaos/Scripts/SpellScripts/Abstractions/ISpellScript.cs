using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

[SuppressMessage("ReSharper", "UnusedParameter.Global")]
public interface ISpellScript : IScript, IDeltaUpdatable
{
    bool CanUse(SpellContext context);
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(SpellContext context);
}