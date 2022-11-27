using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.ItemScripts.Abstractions;

public interface IItemScript : IScript
{
    bool CanUse(Aisling source);
    void OnDropped(Creature source, MapInstance mapInstance);

    void OnEquipped(Aisling aisling);

    void OnPickup(Aisling aisling);
    void OnUnEquipped(Aisling aisling);

    void OnUse(Aisling source);
}