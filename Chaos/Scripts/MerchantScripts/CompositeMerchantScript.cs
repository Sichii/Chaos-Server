using System.Runtime.InteropServices;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeMerchantScript : CompositeScriptBase<IMerchantScript>, IMerchantScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnApproached(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnApproached(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnAttacked(Creature source, int damage)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnAttacked(source, damage);
    }

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
    public virtual void OnDeparture(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDeparture(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnGoldDroppedOn(source, amount);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemDroppedOn(source, slot, count);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnPublicMessage(Creature source, string message) { }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}