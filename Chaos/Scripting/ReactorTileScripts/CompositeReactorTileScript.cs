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
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnClicked(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldDroppedOn(Creature source, Money money)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnGoldDroppedOn(source, money);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldPickedUpFrom(Aisling source, Money money)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnGoldPickedUpFrom(source, money);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemDroppedOn(source, groundItem);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemPickedUpFrom(Aisling source, GroundItem groundItem)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemPickedUpFrom(source, groundItem);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnWalkedOn(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnWalkedOn(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}