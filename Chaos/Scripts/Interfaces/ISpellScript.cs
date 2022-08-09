using Chaos.Objects;
using Chaos.Objects.World;

namespace Chaos.Scripts.Interfaces;

public interface ISpellScript : IScript
{
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(ActivationContext context);
}