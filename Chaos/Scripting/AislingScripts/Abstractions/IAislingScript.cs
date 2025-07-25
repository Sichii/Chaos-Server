#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.CreatureScripts.Abstractions;
#endregion

namespace Chaos.Scripting.AislingScripts.Abstractions;

public interface IAislingScript : ICreatureScript
{
    /// <summary>
    ///     Triggers when an aisling tries to drop an item
    /// </summary>
    /// <param name="item">
    ///     The item the aisling is trying to drop
    /// </param>
    bool CanDropItem(Item item);

    /// <summary>
    ///     Triggers when an aisling tries to drop money
    /// </summary>
    /// <param name="amount">
    ///     The amount of money the player is trying to drop
    /// </param>
    bool CanDropMoney(int amount);

    /// <summary>
    ///     Triggers when an aisling tries to pick up an item
    /// </summary>
    /// <param name="groundItem">
    ///     The ground item the aisling is trying to pick up
    /// </param>
    bool CanPickupItem(GroundItem groundItem);

    /// <summary>
    ///     Triggers when an aisling tries to pick up money
    /// </summary>
    /// <param name="money">
    ///     The money the aisling is trying to pick up
    /// </param>
    bool CanPickupMoney(Money money);

    /// <summary>
    ///     Determines if the specified item can be used by the aisling subject
    /// </summary>
    bool CanUseItem(Item item);

    /// <summary>
    ///     Determines what boards are visible to the aisling when viewing the boards panel
    /// </summary>
    IEnumerable<BoardBase> GetBoardList();

    /// <summary>
    ///     Triggers when the aisling logs in (after they have been added to the world)
    /// </summary>
    void OnLogin();

    /// <summary>
    ///     Triggers when the aisling logs out (before they are removed from the world)
    /// </summary>
    void OnLogout();

    /// <summary>
    ///     Triggers when the aisling's stat is increased
    /// </summary>
    /// <param name="stat">
    ///     The stat that got increased by 1
    /// </param>
    void OnStatIncrease(Stat stat);
}