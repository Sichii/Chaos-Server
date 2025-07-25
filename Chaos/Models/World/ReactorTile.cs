#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Models.World;

public sealed class TemplatedReactorTile : ReactorTile
{
    public ReactorTileTemplate Template { get; }

    /// <inheritdoc />
    public TemplatedReactorTile(
        ReactorTileTemplate template,
        MapInstance mapInstance,
        IPoint point,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys,
        Creature? owner,
        IScript? sourceScript = null)
        : base(
            mapInstance,
            point,
            template.ShouldBlockPathfinding,
            scriptProvider,

            // ReSharper disable once RedundantAssignment
            template.ScriptKeys
                    .Union(extraScriptKeys ??= [])
                    .ToList(),
            template.ScriptVars,
            owner,
            sourceScript)
        => Template = template;
}

public class ReactorTile : InteractableEntity, IDeltaUpdatable, IScripted<IReactorTileScript>
{
    public IScript? SourceScript { get; set; }
    public Creature? Owner { get; }

    /// <inheritdoc />
    public IReactorTileScript Script { get; }

    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    public IDictionary<string, IScriptVars> ScriptVars { get; }

    public bool ShouldBlockPathfinding { get; }

    public ReactorTile(
        MapInstance mapInstance,
        IPoint point,
        bool shouldBlockPathfinding,
        IScriptProvider scriptProvider,

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        ICollection<string> scriptKeys,
        IDictionary<string, IScriptVars> scriptVars,
        Creature? owner = null,
        IScript? sourceScript = null)
        : base(mapInstance, point)
    {
        Owner = owner;
        SourceScript = sourceScript;
        ShouldBlockPathfinding = shouldBlockPathfinding;
        ScriptVars = new Dictionary<string, IScriptVars>(scriptVars, StringComparer.OrdinalIgnoreCase);
        ScriptKeys = new HashSet<string>(scriptKeys, StringComparer.OrdinalIgnoreCase);
        Script = scriptProvider.CreateScript<IReactorTileScript, ReactorTile>(ScriptKeys, this);
    }

    public void Update(TimeSpan elapsed) => Script.Update(elapsed);

    public override void OnClicked(Aisling source) => Script.OnClicked(source);

    public void OnGoldDroppedOn(Creature source, Money money) => Script.OnGoldDroppedOn(source, money);

    public void OnGoldPickedUpFrom(Aisling source, Money money) => Script.OnGoldPickedUpFrom(source, money);

    public void OnItemDroppedOn(Creature source, GroundItem groundItem) => Script.OnItemDroppedOn(source, groundItem);

    public void OnItemPickedUpFrom(Aisling source, GroundItem groundItem, int originalCount)
        => Script.OnItemPickedUpFrom(source, groundItem, originalCount);

    public void OnWalkedOn(Creature source) => Script.OnWalkedOn(source);
}