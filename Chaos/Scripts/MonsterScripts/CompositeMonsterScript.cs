using System.Runtime.InteropServices;
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
    public virtual void OnDeath()
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDeath();
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
    public virtual void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemDroppedOn(source, item);
    }

    /// <inheritdoc />
    public virtual void OnSpawn()
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnSpawn();
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.Update(delta);
    }
}