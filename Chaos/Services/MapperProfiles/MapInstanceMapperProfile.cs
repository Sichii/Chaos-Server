using Chaos.Containers;
using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class MapInstanceMapperProfile : IMapperProfile<MapInstance, MapInstanceSchema>,
                                               IMapperProfile<MapTemplate, MapTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly IShardGenerator ShardGenerator;
    private readonly ISimpleCache SimpleCache;

    public MapInstanceMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ITypeMapper mapper,
        IShardGenerator shardGenerator
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Mapper = mapper;
        ShardGenerator = shardGenerator;
    }

    public MapInstance Map(MapInstanceSchema obj)
    {
        var template = SimpleCache.Get<MapTemplate>(obj.TemplateKey);

        var mapInstance = new MapInstance(
            template,
            SimpleCache,
            ShardGenerator,
            ScriptProvider,
            obj.Name,
            obj.InstanceId,
            obj.ScriptKeys)
        {
            Flags = obj.Flags,
            Music = obj.Music,
            MinimumLevel = obj.MinimumLevel,
            MaximumLevel = obj.MaximumLevel,
            ShardingOptions = obj.ShardingOptions == null ? null : Mapper.Map<ShardingOptions>(obj.ShardingOptions)
        };

        foreach (var doorTemplate in template.Doors.Values)
        {
            var door = new Door(doorTemplate, mapInstance);
            mapInstance.SimpleAdd(door);
        }

        return mapInstance;
    }

    public MapInstanceSchema Map(MapInstance obj) => throw new NotImplementedException();

    public MapTemplate Map(MapTemplateSchema obj) => new()
    {
        Width = obj.Width,
        Height = obj.Height,
        Bounds = new Rectangle(
            0,
            0,
            obj.Width,
            obj.Height),
        TemplateKey = obj.TemplateKey,
        WarpPoints = obj.WarpPoints,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        Tiles = new Tile[obj.Width, obj.Height]
    };

    public MapTemplateSchema Map(MapTemplate obj) => throw new NotImplementedException();
}