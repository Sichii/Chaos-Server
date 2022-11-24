using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts;

public class ShowWorldMapScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected string WorldMapKey { get; init; } = null!;
    
    /// <inheritdoc />
    public ShowWorldMapScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var worldMap = SimpleCache.Get<WorldMap>(WorldMapKey);

        //if we cant set the active object, return
        if (!aisling.ActiveObject.SetIfNull(worldMap))
            return;
        
        aisling.MapInstance.RemoveObject(source);
        aisling.Client.SendWorldMap(worldMap);
    }
}