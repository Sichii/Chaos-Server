using System.Runtime.InteropServices;
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
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDropped(source, mapInstance);
    }

    /// <inheritdoc />
    public virtual void OnEquipped(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnEquipped(aisling);
    }

    /// <inheritdoc />
    public virtual void OnPickup(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnPickup(aisling);
    }

    /// <inheritdoc />
    public virtual void OnUnEquipped(Aisling aisling)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUnEquipped(aisling);
    }

    /// <inheritdoc />
    public virtual void OnUse(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnUse(source);
    }
}