using Chaos.Containers;
using Chaos.Data;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.World;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class MapInstanceMapperProfile : IMapperProfile<MapInstance, MapInstanceSchema>
{
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptProvider ScriptProvider;

    public MapInstanceMapperProfile(ISimpleCache simpleCache, IScriptProvider scriptProvider)
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
    }

    public MapInstance Map(MapInstanceSchema obj)
    {
        var template = SimpleCache.GetObject<MapTemplate>(obj.TemplateKey);

        var mapInstance = new MapInstance(
            template,
            ScriptProvider,
            obj.Name,
            obj.InstanceId,
            obj.ScriptKeys)
        {
            Flags = obj.Flags,
            Music = obj.Music
        };

        foreach (var doorTemplate in template.Doors.Values)
        {
            var door = new Door(doorTemplate, mapInstance);
            mapInstance.SimpleAdd(door);
        }

        foreach (var warpSchema in obj.Warps)
        {
            var warp = new Warp
            {
                SourceLocation = new Location(obj.InstanceId, warpSchema.Source.X, warpSchema.Source.Y),
                TargetLocation = new Location(warpSchema.Destination.Map, warpSchema.Destination.X, warpSchema.Destination.Y)
            };
            
            var warpTile = new WarpTile(mapInstance, SimpleCache, warp);
            mapInstance.SimpleAdd(warpTile);
        }

        return mapInstance;
    }

    public MapInstanceSchema Map(MapInstance obj) => throw new NotImplementedException();
}