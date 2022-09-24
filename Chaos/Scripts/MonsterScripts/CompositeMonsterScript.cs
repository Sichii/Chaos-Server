using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

public class CompositeMonsterScript : CompositeScriptBase<IMonsterScript>, IMonsterScript
{
    protected Monster Subject { get; }

    public CompositeMonsterScript(Monster subject) => Subject = subject;

    /// <inheritdoc />
    public void OnApproached(Creature source)
    {
        foreach (var component in Components)
            component.OnApproached(source);
    }

    /// <inheritdoc />
    public void OnAttacked(Creature source, ref int damage)
    {
        foreach (var component in Components)
            component.OnAttacked(source, ref damage);
    }

    /// <inheritdoc />
    public void OnClicked(Aisling source)
    {
        foreach (var component in Components)
            component.OnClicked(source);
    }

    /// <inheritdoc />
    public void OnDeath()
    {
        foreach (var component in Components)
            component.OnDeath();
    }

    /// <inheritdoc />
    public void OnDeparture(Creature source)
    {
        foreach (var component in Components)
            component.OnDeparture(source);
    }

    /// <inheritdoc />
    public void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (var component in Components)
            component.OnGoldDroppedOn(source, amount);
    }

    /// <inheritdoc />
    public void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (var component in Components)
            component.OnItemDroppedOn(source, item);
    }

    /// <inheritdoc />
    public void OnSpawn()
    {
        foreach (var component in Components)
            component.OnSpawn();
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        foreach (var component in Components)
            component.Update(delta);
    }
}