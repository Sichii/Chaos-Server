using System.Runtime.InteropServices;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeReactorTileScript : CompositeScriptBase<IReactorTileScript>, IReactorTileScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnClicked(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnClicked(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldDroppedOn(Creature source, Money money)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnGoldDroppedOn(source, money);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldPickedUpFrom(Aisling source, Money money)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnGoldPickedUpFrom(source, money);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnItemDroppedOn(source, groundItem);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemPickedUpFrom(Aisling source, GroundItem groundItem, int originalCount)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnItemPickedUpFrom(source, groundItem, originalCount);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnWalkedOn(Creature source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnWalkedOn(source);
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