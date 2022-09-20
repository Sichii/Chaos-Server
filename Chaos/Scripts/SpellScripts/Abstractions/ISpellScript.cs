using Chaos.Objects;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public interface ISpellScript : IScript
{
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(ActivationContext context);
}