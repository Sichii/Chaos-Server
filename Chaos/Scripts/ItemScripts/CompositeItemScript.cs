using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    protected Item Subject { get; }

    public CompositeItemScript(Item subject) => Subject = subject;

    public void OnDropped(Creature source, MapInstance mapInstance)
    {
        foreach (var component in Components)
            component.OnDropped(source, mapInstance);
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

    public void OnUse(Aisling source)
    {
        var exchange = source.ActiveObject.TryGet<Exchange>();

        if (exchange != null)
        {
            exchange.AddItem(source, Subject.Slot);

            return;
        }

        foreach (var component in Components)
            component.OnUse(source);
    }
}