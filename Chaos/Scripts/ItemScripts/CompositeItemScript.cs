using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    /// <inheritdoc />
    public virtual void OnDropped(Creature source, MapInstance mapInstance)
    {
        foreach (var component in Components)
            component.OnDropped(source, mapInstance);
    }

    /// <inheritdoc />
    public virtual void OnEquipped(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnEquipped(aisling);
    }

    /// <inheritdoc />
    public virtual void OnPickup(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnPickup(aisling);
    }

    /// <inheritdoc />
    public virtual void OnUnEquipped(Aisling aisling)
    {
        foreach (var component in Components)
            component.OnUnEquipped(aisling);
    }

    /// <inheritdoc />
    public virtual void OnUse(Aisling source)
    {
        foreach (var component in Components)
            component.OnUse(source);
    }
}