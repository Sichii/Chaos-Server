#region
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time.Abstractions;
#endregion

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.ItemScripts.Abstractions;

public interface IItemScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Determines if the item subject can be used by the specified aisling (check level, class, etc)
    /// </summary>
    bool CanUse(Aisling source);

    /// <summary>
    ///     Triggers when the item subject is dropped on the ground
    /// </summary>
    /// <param name="source">
    ///     The creature that dropped the item
    /// </param>
    /// <param name="mapInstance">
    ///     The map the item was dropped on
    /// </param>
    /// <remarks>
    ///     Comes before <see cref="IReactorTileScript.OnItemDroppedOn" />
    /// </remarks>
    void OnDropped(Creature source, MapInstance mapInstance);

    /// <summary>
    ///     Triggers when the item subject has been equipped by the aisling
    /// </summary>
    void OnEquipped(Aisling aisling);

    /// <summary>
    ///     Triggers when the item subject is picked up by the specified aisling
    /// </summary>
    /// <param name="aisling">
    ///     The aisling that picked the item up
    /// </param>
    /// <param name="originalItem">
    ///     The original state of the item before it was picked up
    /// </param>
    /// <param name="originalCount">
    ///     The original count the item had before it was picked up
    /// </param>
    /// <remarks>
    ///     Comes before <see cref="IReactorTileScript.OnItemPickedUpFrom" />
    ///     <br />
    ///     When an item is picked up, it may be absorbed into an existing stack in the inventory. When this happens, this
    ///     event is fired from the final item stack. The original item and count are provided for reference.
    /// </remarks>
    void OnPickup(Aisling aisling, Item originalItem, int originalCount);

    /// <summary>
    ///     Triggers when the item subject is unequipped by the aisling
    /// </summary>
    void OnUnEquipped(Aisling aisling);

    /// <summary>
    ///     Triggers when the item subject is used by the specified aisling
    /// </summary>
    /// <remarks>
    ///     If you want to determine if the item can be used, use <see cref="CanUse" /> instead
    /// </remarks>
    void OnUse(Aisling source);
}