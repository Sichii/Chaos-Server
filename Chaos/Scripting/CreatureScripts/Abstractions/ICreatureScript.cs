#region
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.CreatureScripts.Abstractions;

public interface ICreatureScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Determines if the specified item can be dropped on the creature subject by the source aisling
    /// </summary>
    /// <param name="source">
    ///     The aisling trying to drop the item on the creature source
    /// </param>
    /// <param name="item">
    ///     The item being dropped
    /// </param>
    bool CanDropItemOn(Aisling source, Item item);

    /// <summary>
    ///     Determines if the creature subject is allowed to move (check for paralysis, sleep, etc)
    /// </summary>
    bool CanMove();

    /// <summary>
    ///     Determines if the creature subject can see the specified entity
    /// </summary>
    bool CanSee(VisibleEntity entity);

    /// <summary>
    ///     Determines if the creature subject is allowed to talk. (check for silence, mute, etc)
    /// </summary>
    bool CanTalk();

    /// <summary>
    ///     Determines if the creature subject is allowed to turn (check for paralysis, sleep, etc)
    /// </summary>
    bool CanTurn();

    /// <summary>
    ///     Determines if the creature subject is allowed to use the specified skill (check for paralysis, sleep, etc)
    /// </summary>
    bool CanUseSkill(Skill skill);

    /// <summary>
    ///     Determines if the creature subject is allowed to use the specified spell (check for paralysis, sleep, etc)
    /// </summary>
    bool CanUseSpell(Spell spell);

    /// <summary>
    ///     Determines if the creature subject is friently to the specified creature (often for the purposes of acquiring
    ///     targets of beneficial abilities)
    /// </summary>
    bool IsFriendlyTo(Creature creature);

    /// <summary>
    ///     Determines if the creature subject is hostile to the specified creature (often for the purposes of acquiring
    ///     targets of hostile abilities)
    /// </summary>
    /// <param name="creature">
    /// </param>
    /// <returns>
    /// </returns>
    bool IsHostileTo(Creature creature);

    /// <summary>
    ///     Triggers when the specified creature becomes observable to the creature subject
    /// </summary>
    void OnApproached(Creature source);

    /// <summary>
    ///     Triggers when the specified creature attacks the creature subject
    /// </summary>
    /// <param name="source">
    ///     The source of the attack
    /// </param>
    /// <param name="damage">
    ///     The final amount of damage dealt
    /// </param>
    void OnAttacked(Creature source, int damage);

    /// <summary>
    ///     Triggers when the creature subject is clicked by the specified aisling
    /// </summary>
    void OnClicked(Aisling source);

    /// <summary>
    ///     Triggers when the creature subject dies. (Triggered by ApplyDamageScripts)
    /// </summary>
    /// <remarks>
    ///     Comes after <see cref="OnAttacked" />
    /// </remarks>
    void OnDeath();

    /// <summary>
    ///     Triggers when the specified creature is no longer observable to the creature subject
    /// </summary>
    void OnDeparture(Creature source);

    /// <summary>
    ///     Triggers when the specified aisling drops gold on the creature subject
    /// </summary>
    /// <param name="source">
    ///     The aisling dropping gold on the creature subject
    /// </param>
    /// <param name="amount">
    ///     The amount of gold dropped
    /// </param>
    void OnGoldDroppedOn(Aisling source, int amount);

    /// <summary>
    ///     Triggers when the specified creature heals the creature subject
    /// </summary>
    /// <param name="source">
    ///     The source of healing
    /// </param>
    /// <param name="healing">
    ///     The final amount healed
    /// </param>
    void OnHealed(Creature source, int healing);

    /// <summary>
    ///     Triggers when the specified aisling drops an item on the creature subject
    /// </summary>
    /// <param name="source">
    ///     The aisling that dropped the item on the creature subject
    /// </param>
    /// <param name="item">
    ///     The item being dropped
    /// </param>
    /// <remarks>
    ///     At this point, the item has already been removed from the aisling's inventory. A check was made via
    ///     <see cref="CanDropItemOn" /> to ensure the item could be dropped. This method merely determines what to do with
    ///     that item.
    /// </remarks>
    void OnItemDroppedOn(Aisling source, Item item);

    /// <summary>
    ///     Triggers when the specified creature sends a public message
    /// </summary>
    /// <param name="source">
    ///     The source of the public message
    /// </param>
    /// <param name="message">
    ///     The message sent by the source
    /// </param>
    void OnPublicMessage(Creature source, string message);
}