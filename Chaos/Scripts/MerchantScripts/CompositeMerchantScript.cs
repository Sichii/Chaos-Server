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
        foreach (var component in Components)
            component.OnApproached(source);
    }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, ref int damage)
    {
        foreach (var component in Components)
            component.OnAttacked(source, ref damage);
    }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source)
    {
        foreach (var component in Components)
            component.OnClicked(source);
    }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source)
    {
        foreach (var component in Components)
            component.OnDeparture(source);
    }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (var component in Components)
            component.OnGoldDroppedOn(source, amount);
    }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        foreach (var component in Components)
            component.OnItemDroppedOn(source, slot, count);
    }

    /// <inheritdoc />
    public void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (var component in Components)
            component.Update(delta);
    }
}