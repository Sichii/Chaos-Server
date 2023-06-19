using System.Runtime.InteropServices;
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUse(Aisling source)
    {
        var canUse = true;

        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            canUse &= script.CanUse(source);

        return canUse;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDropped(Creature source, MapInstance mapInstance)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnDropped(source, mapInstance);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnEquipped(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnEquipped(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnPickup(Aisling aisling, Item originalItem, int originalCount)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnPickup(aisling, originalItem, originalCount);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnUnEquipped(Aisling aisling)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnUnEquipped(aisling);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnUse(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnUse(source);
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}