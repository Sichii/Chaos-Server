#region
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Time.Abstractions;
#endregion

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public interface IReactorTileScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Triggers when the reactor tile is clicked by an Aisling (for reference, only tiles with foregrounds can be clicked)
    /// </summary>
    void OnClicked(Aisling source);

    /// <summary>
    ///     Triggers when gold is dropped on the reactor tile by a creature (intentionally or as part of death drops)
    /// </summary>
    /// <param name="source">
    ///     The creature that dropped the gold
    /// </param>
    /// <param name="money">
    ///     The money object(ground object for gold) placed on the tile
    /// </param>
    void OnGoldDroppedOn(Creature source, Money money);

    /// <summary>
    ///     Triggers when gold is picked up from the reactor tile by an Aisling
    /// </summary>
    /// <param name="source">
    ///     The aisling that picked up the gold
    /// </param>
    /// <param name="money">
    ///     The money object(ground object for gold) that got picked up
    /// </param>
    void OnGoldPickedUpFrom(Aisling source, Money money);

    /// <summary>
    ///     Triggers when an item is dropped on the reactor tile by a creature (intentionally or as part of death drops)
    /// </summary>
    /// <param name="source">
    ///     The creature that dropped the item
    /// </param>
    /// <param name="groundItem">
    ///     The ground item placed on the tile
    /// </param>
    /// <remarks>
    ///     Comes after <see cref="IItemScript.OnDropped" />
    /// </remarks>
    void OnItemDroppedOn(Creature source, GroundItem groundItem);

    /// <summary>
    ///     Triggers when an item is picked up from the reactor tile by an Aisling
    /// </summary>
    /// <param name="source">
    ///     The creature that picked up the item
    /// </param>
    /// <param name="groundItem">
    ///     The ground item that got picked up
    /// </param>
    /// <param name="originalCount">
    ///     The count of the item before it may have been merged into a stack
    /// </param>
    /// <remarks>
    ///     Comes after <see cref="IItemScript.OnPickup" />
    ///     <br />
    ///     When an item is picked up, it may be absorbed into an existing stack in the inventory. When this happens, this
    ///     method is called with the original ground item and it's count as reference. The stack that got merged into is not
    ///     available.
    /// </remarks>
    void OnItemPickedUpFrom(Aisling source, GroundItem groundItem, int originalCount);

    /// <summary>
    ///     Triggers when a creature walks on the reactor tile
    /// </summary>
    void OnWalkedOn(Creature source);
}