using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.SpellScripts.Abstractions;

[SuppressMessage("ReSharper", "UnusedParameter.Global")]
public interface ISpellScript : IScript, IDeltaUpdatable
{
    bool CanUse(SpellContext context);
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(SpellContext context);
}