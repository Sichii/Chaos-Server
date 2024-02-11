using System.Runtime.InteropServices;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
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
    /// <param name="entity">
    /// </param>
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
    public virtual bool CanUseItem(Item item)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (!script.CanUseItem(item))
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
    public virtual IEnumerable<BoardBase> GetBoardList()
    {
        //cant use CollectionMarshal.AsSpan here because of yield
        foreach (var script in Scripts)
            foreach (var board in script.GetBoardList())
                yield return board;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool IsBlind()
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (script.IsBlind())
                return true;

        return false;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool IsFriendlyTo(Creature creature)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (script.IsFriendlyTo(creature))
                return true;

        return false;
    }

    /// <summary>
    ///     DO NOT EDIT THIS SCRIPT
    /// </summary>
    public virtual bool IsHostileTo(Creature creature)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            if (script.IsHostileTo(creature))
                return true;

        return false;
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
    public virtual void OnDeath()
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
    public void OnStatIncrease(Stat stat)
    {
        foreach (ref var script in CollectionsMarshal.AsSpan(Scripts))
            script.OnStatIncrease(stat);
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