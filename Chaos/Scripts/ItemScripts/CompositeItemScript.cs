using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;

namespace Chaos.Scripts.ItemScripts;

public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    protected Item Source { get; }

    public CompositeItemScript(Item source) => Source = source;

    public void OnUnequip(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnUnequip(aisling);
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