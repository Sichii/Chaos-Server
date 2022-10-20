using System.Runtime.InteropServices;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class CompositeMerchantScript : CompositeScriptBase<IMerchantScript>, IMerchantScript
{
    public CompositeMerchantScript(Merchant subject) => Subject = subject;
    protected Merchant Subject { get; }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        foreach(var component in Components)
            component.Update(delta);
    }

    /// <inheritdoc />
    public void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        foreach(var component in Components)
            component.OnItemDroppedOn(source, slot, count);
    }

    /// <inheritdoc />
    public void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach(var component in Components)
            component.OnGoldDroppedOn(source, amount);
    }

    /// <inheritdoc />
    public void OnApproached(Creature source)
    {
        foreach(var component in Components)
            component.OnApproached(source);
    }

    /// <inheritdoc />
    public void OnClicked(Aisling source)
    {
        foreach(var component in Components)
            component.OnClicked(source);
    }

    /// <inheritdoc />
    public void OnDeparture(Creature source)
    {
        foreach(var component in Components)
            component.OnDeparture(source);
    }

    /// <inheritdoc />
    public void OnAttacked(Creature source, ref int damage)
    {
        foreach(var component in Components)
            component.OnAttacked(source, ref damage);
    }
}