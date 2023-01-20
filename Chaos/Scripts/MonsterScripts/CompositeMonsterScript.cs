using System.Runtime.InteropServices;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeMonsterScript : CompositeScriptBase<IMonsterScript>, IMonsterScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanMove() => Components.All(component => component.CanMove());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTalk() => Components.All(component => component.CanTalk());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTurn() => Components.All(component => component.CanTurn());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSkill(Skill skill) => Components.All(component => component.CanUseSkill(skill));

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSpell(Spell spell) => Components.All(component => component.CanUseSpell(spell));

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
    public virtual void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnAttacked(source, damage);
    }

    /// <inheritdoc />
    public void OnAttacked(Creature attacker, int damage) => OnAttacked(attacker, damage, null);

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
    public virtual void OnDeath()
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnDeath();
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
    public virtual void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnItemDroppedOn(source, item);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnSpawn()
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Components))
            component.OnSpawn();
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