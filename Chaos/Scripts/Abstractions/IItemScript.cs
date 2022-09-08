using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Abstractions;

public interface IItemScript : IScript
{
    void OnDropped(Creature source, MapInstance mapInstance);

    void OnEquipped(Aisling aisling);

    void OnPickup(Aisling aisling);
    void OnUnEquipped(Aisling aisling);

    void OnUse(Aisling source);
}