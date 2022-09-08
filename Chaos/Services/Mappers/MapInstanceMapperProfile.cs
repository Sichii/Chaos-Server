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
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptProvider ScriptProvider;

    public MapInstanceMapperProfile(ISimpleCache simpleCache, ITypeMapper mapper, IScriptProvider scriptProvider)
    {
        SimpleCache = simpleCache;
        Mapper = mapper;
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

        foreach (var warp in Mapper.MapMany<Warp>(obj.Warps))
        {
            var warpTile = new WarpTile(warp, SimpleCache);
            mapInstance.SimpleAdd(warpTile);
        }

        return mapInstance;
    }

    public MapInstanceSchema Map(MapInstance obj) => throw new NotImplementedException();
}