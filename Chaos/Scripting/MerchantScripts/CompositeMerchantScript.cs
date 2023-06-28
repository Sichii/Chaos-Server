using System.Runtime.InteropServices;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

/// <summary>
///     DO NOT EDIT THIS SCRIPT
/// </summary>
public class CompositeMerchantScript : CompositeScriptBase<IMerchantScript>, IMerchantScript
{
    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanMove()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanMove())
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanSee(VisibleEntity entity)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanSee(entity))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTalk()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanTalk())
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanTurn()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanTurn())
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSkill(Skill skill)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanUseSkill(skill))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool CanUseSpell(Spell spell)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanUseSpell(spell))
                return false;

        return true;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnApproached(Creature source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnApproached(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnAttacked(Creature source, int damage)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnAttacked(source, damage);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnClicked(Aisling source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnClicked(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public void OnDeath()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnDeath();
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnDeparture(Creature source)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnDeparture(source);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnGoldDroppedOn(source, amount);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnHealed(Creature source, int healing)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnHealed(source, healing);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnItemDroppedOn(Aisling source, Item item)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnItemDroppedOn(source, item);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void OnPublicMessage(Creature source, string message)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnPublicMessage(source, message);
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual void Update(TimeSpan delta)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.Update(delta);
    }
}