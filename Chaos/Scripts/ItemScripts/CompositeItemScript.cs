using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.ItemScripts;

public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    protected Item Source { get; }

    public CompositeItemScript(Item source) => Source = source;

    public void OnDropped(Creature creature, MapInstance mapInstance)
    {
        foreach (var component in Components)
            component.OnDropped(creature, mapInstance);
    }

    public void OnEquipped(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnEquipped(aisling);
    }

    public void OnPickup(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnPickup(aisling);
    }

    public void OnUnEquipped(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnUnEquipped(aisling);
    }

    public void OnUse(Aisling aisling)
    {
        var exchange = aisling.ActiveObject.TryGet<Exchange>();

        if (exchange != null)
        {
            exchange.AddItem(aisling, Source.Slot);

            return;
        }

        foreach (var component in Components)
            component.OnUse(aisling);
    }
}