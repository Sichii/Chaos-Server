using Chaos.Objects.World;

namespace Chaos.Scripts.Interfaces;

public interface IItemScript : IScript
{
    void OnUnequip(Aisling aisling);

    void OnUse(Aisling aisling);
}