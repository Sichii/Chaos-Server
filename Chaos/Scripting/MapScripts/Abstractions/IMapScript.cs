#region
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.CreatureScripts.Abstractions;
using Chaos.Time.Abstractions;
#endregion

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.MapScripts.Abstractions;

public interface IMapScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Triggers when a creature enters the map (whether by spawning or entering via warp)
    /// </summary>
    /// <remarks>
    ///     Comes before <see cref="ICreatureScript.OnApproached" />
    /// </remarks>
    void OnEntered(Creature creature);

    /// <summary>
    ///     Triggers when a creature exits the map (whether by death or leaving via warp)
    /// </summary>
    /// <remarks>
    ///     Comes after <see cref="ICreatureScript.OnDeparture" />
    /// </remarks>
    void OnExited(Creature creature);

    /// <summary>
    ///     Triggers when the map is morphed (map template has changed)
    /// </summary>
    /// <remarks>
    ///     Comes after <see cref="OnMorphing" />
    ///     <br />
    ///     At this point, the map has already been morphed and the new map template is in place
    /// </remarks>
    void OnMorphed();

    /// <summary>
    ///     Triggers when the map is morphing (map template is about to change)
    /// </summary>
    /// <param name="newMapTemplate">
    ///     The map template this map instance is changing to
    /// </param>
    /// <remarks>
    ///     Comes before <see cref="OnMorphed" />
    ///     <br />
    ///     At this point, the map has not yet been morphed and the new map template is provided for reference
    /// </remarks>
    void OnMorphing(MapTemplate newMapTemplate);
}