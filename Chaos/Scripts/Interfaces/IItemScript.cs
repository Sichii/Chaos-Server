using Chaos.Objects.World;

namespace Chaos.Scripts.Interfaces;

public interface IItemScript : IScript
{
    void OnUnequip(User user);

    void OnUse(User user);
}