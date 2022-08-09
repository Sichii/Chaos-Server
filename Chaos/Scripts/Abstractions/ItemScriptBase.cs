using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.Abstractions;

public abstract class ItemScriptBase : ScriptBase, IItemScript
{
    protected Item Source { get; }

    protected ItemScriptBase(Item item) => Source = item;
    public virtual void OnDropped(Creature creature, MapInstance mapInstance) { }
    public virtual void OnEquipped(Aisling aisling) { }
    public virtual void OnPickup(Aisling aisling) { }

    public virtual void OnUnEquipped(Aisling aisling) { }

    public virtual void OnUse(Aisling aisling) { }
}