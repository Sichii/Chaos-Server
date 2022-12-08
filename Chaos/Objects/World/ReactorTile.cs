using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;

namespace Chaos.Objects.World;

public sealed class ReactorTile : MapEntity, IScripted<IReactorTileScript>
{
    /// <inheritdoc />
    public IReactorTileScript Script { get; }
    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    public IDictionary<string, DynamicVars> ScriptVars { get; }

    public bool ShouldBlockPathfinding { get; }

    public ReactorTile(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        IScriptProvider scriptProvider,
        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        ICollection<string> scriptKeys,
        IDictionary<string, DynamicVars> scriptVars
    )
        : base(mapInstance, point)
    {
        ShouldBlockPathfinding = shouldBlockPathfinding;
        ScriptVars = new Dictionary<string, DynamicVars>(scriptVars, StringComparer.OrdinalIgnoreCase);
        ScriptKeys = new HashSet<string>(scriptKeys, StringComparer.OrdinalIgnoreCase);
        Script = scriptProvider.CreateScript<IReactorTileScript, ReactorTile>(ScriptKeys, this);
    }

    public void OnClicked(Aisling source) => Script.OnClicked(source);

    public void OnGoldDroppedOn(Creature source, Money money) => Script.OnGoldDroppedOn(source, money);

    public void OnGoldPickedUpFrom(Aisling source, Money money) => Script.OnGoldPickedUpFrom(source, money);

    public void OnItemDroppedOn(Creature source, GroundItem groundItem) => Script.OnItemDroppedOn(source, groundItem);

    public void OnItemPickedUpFrom(Aisling source, GroundItem groundItem) => Script.OnItemPickedUpFrom(source, groundItem);

    public void OnWalkedOn(Creature source) => Script.OnWalkedOn(source);

    public void Update(TimeSpan elapsed) => Script.Update(elapsed);
}