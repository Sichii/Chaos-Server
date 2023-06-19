using System.Runtime.InteropServices;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;

namespace Chaos.Scripting.AislingScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeAislingScript : CompositeScriptBase<IAislingScript>, IAislingScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanMove() => Scripts.All(component => component.CanMove());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    /// <param name="entity"></param>
    public virtual bool CanSee(VisibleEntity entity) => Scripts.All(component => component.CanSee(entity));

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTalk() => Scripts.All(component => component.CanTalk());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTurn() => Scripts.All(component => component.CanTurn());

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseItem(Item item) => Scripts.All(component => component.CanUseItem(item));

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSkill(Skill skill) => Scripts.All(component => component.CanUseSkill(skill));

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSpell(Spell spell) => Scripts.All(component => component.CanUseSpell(spell));

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnApproached(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnApproached(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnAttacked(Creature source, int damage)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnAttacked(source, damage);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnClicked(Aisling source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnClicked(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDeath()
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnDeath();
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDeparture(Creature source)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnDeparture(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnGoldDroppedOn(source, amount);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnHealed(Creature source, int healing)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnHealed(source, healing);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnItemDroppedOn(source, item);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnPublicMessage(Creature source, string message)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.OnPublicMessage(source, message);
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var component in CollectionsMarshal.AsSpan(Scripts))
            component.Update(delta);
    }
}