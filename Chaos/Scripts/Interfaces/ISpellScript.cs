using Chaos.Objects;

namespace Chaos.Scripts.Interfaces;

public interface ISpellScript : IScript
{
    void OnUse(ActivationContext context);
}