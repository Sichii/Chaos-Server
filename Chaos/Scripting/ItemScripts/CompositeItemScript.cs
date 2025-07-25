#region
using System.Runtime.InteropServices;
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
#endregion

namespace Chaos.Scripting.ItemScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeItemScript : CompositeScriptBase<IItemScript>, IItemScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanBeDropped(Aisling source, Point targetPoint)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanBeDropped(source, targetPoint))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanBeDroppedOn(Aisling source, Creature creature)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanBeDroppedOn(source, creature))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanBePickedUp(Aisling source, Point sourcePoint)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanBePickedUp(source, sourcePoint))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUse(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanUse(source))
                return false;

        return true;
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
    public virtual void OnNotepadTextUpdated(Aisling source, string? oldText)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnNotepadTextUpdated(source, oldText);
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

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}