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
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public MapInstanceMapperProfile(ISimpleCache simpleCache, IScriptProvider scriptProvider, ITypeMapper mapper)
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Mapper = mapper;
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
            Music = obj.Music,
            MinimumLevel = obj.MinimumLevel,
            MaximumLevel = obj.MaximumLevel
        };

        foreach (var doorTemplate in template.Doors.Values)
        {
            var door = new Door(doorTemplate, mapInstance);
            mapInstance.SimpleAdd(door);
        }

        foreach (var warp in Mapper.MapMany<Warp>(obj.Warps))
        {
            var warpTile = new WarpTile(mapInstance, SimpleCache, warp);
            mapInstance.SimpleAdd(warpTile);
        }

        return mapInstance;
    }

    public MapInstanceSchema Map(MapInstance obj) => throw new NotImplementedException();
}