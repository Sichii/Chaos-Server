using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

public class CompositeMonsterScript : CompositeScriptBase<IMonsterScript>, IMonsterScript
{
    protected Monster Source { get; }
 
    public CompositeMonsterScript(Monster source) => Source = source;
    
    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        foreach(var component in Components)
            component.Update(delta);
    }

    /// <inheritdoc />
    public void OnSpawn()
    {
        foreach(var component in Components)
            component.OnSpawn();
    }

    /// <inheritdoc />
    public void OnApproached(Creature source)
    {
        foreach(var component in Components)
            component.OnApproached(source);
    }

    /// <inheritdoc />
    public void OnDeparture(Creature source)
    {
        foreach(var component in Components)
            component.OnDeparture(source);
    }

    /// <inheritdoc />
    public void OnDeath(Creature source)
    {
        foreach(var component in Components)
            component.OnDeath(source);
    }

    /// <inheritdoc />
    public void OnAttacked(Creature source, int damage)
    {
        foreach(var component in Components)
            component.OnAttacked(source, damage);
    }

    /// <inheritdoc />
    public void OnClicked(Aisling source)
    {
        foreach(var component in Components)
            component.OnClicked(source);
    }

    /// <inheritdoc />
    public void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach(var component in Components)
            component.OnItemDroppedOn(source, item);
    }

    /// <inheritdoc />
    public void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach(var component in Components)
            component.OnGoldDroppedOn(source, amount);
    }
}