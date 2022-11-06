using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

public class CompositeMonsterScript : CompositeScriptBase<IMonsterScript>, IMonsterScript
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
    public virtual void OnDeath()
    {
        foreach (var component in Components)
            component.OnDeath();
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
    public virtual void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (var component in Components)
            component.OnItemDroppedOn(source, item);
    }

    /// <inheritdoc />
    public virtual void OnSpawn()
    {
        foreach (var component in Components)
            component.OnSpawn();
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (var component in Components)
            component.Update(delta);
    }
}