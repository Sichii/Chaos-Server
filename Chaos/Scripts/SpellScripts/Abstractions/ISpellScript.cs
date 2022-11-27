using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public interface ISpellScript : IScript
{
    bool CanUse(SpellContext context);
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(SpellContext context);
}