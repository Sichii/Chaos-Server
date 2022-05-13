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
    
    public void OnUnequip(User user)
    {
        foreach (var component in Components)
            component.OnUnequip(user);
    }

    public void OnUse(User user)
    {
        var exchange = user.ActiveObject.TryGet<Exchange>();

        if (exchange != null)
        {
            exchange.AddItem(user, Source.Slot);

            return;
        }
        
        foreach (var component in Components)
            component.OnUse(user);
    }
}