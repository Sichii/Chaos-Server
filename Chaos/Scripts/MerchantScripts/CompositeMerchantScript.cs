using System.Runtime.InteropServices;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class CompositeMerchantScript : CompositeScriptBase<IMerchantScript>, IMerchantScript
{
    /// <inheritdoc />
    public virtual void OnApproached(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnApproached(source);
    }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, ref int damage)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnAttacked(source, ref damage);
    }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnClicked(source);
    }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDeparture(source);
    }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnGoldDroppedOn(source, amount);
    }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemDroppedOn(source, slot, count);
    }

    /// <inheritdoc />
    public void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}