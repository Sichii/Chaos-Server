#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Scripting.CreatureScripts.Abstractions;
#endregion

namespace Chaos.Scripting.AislingScripts.Abstractions;

public interface IAislingScript : ICreatureScript
{
    /// <summary>
    ///     Determines if the specified item can be used by the aisling subject
    /// </summary>
    bool CanUseItem(Item item);

    /// <summary>
    ///     Determines what boards are visible to the aisling when viewing the boards panel
    /// </summary>
    IEnumerable<BoardBase> GetBoardList();

    /// <summary>
    ///     Triggers when the aisling's stat is increased
    /// </summary>
    /// <param name="stat">
    ///     The stat that got increased by 1
    /// </param>
    void OnStatIncrease(Stat stat);
}