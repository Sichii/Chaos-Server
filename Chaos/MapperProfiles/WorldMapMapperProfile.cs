using Chaos.Containers;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class WorldMapMapperProfile : IMapperProfile<WorldMap, WorldMapSchema>
{
    private readonly ISimpleCache SimpleCache;
    
    public WorldMapMapperProfile(ISimpleCache simpleCache) => SimpleCache = simpleCache;

    /// <inheritdoc />
    public WorldMap Map(WorldMapSchema obj) => new WorldMap
    {
        WorldMapKey = obj.WorldMapKey,
        FieldIndex = obj.FieldIndex,
        Nodes = obj.NodeKeys.Select(key => SimpleCache.Get<WorldMapNode>(key))
                   .ToDictionary(node => node.UniqueId)
    };

    /// <inheritdoc />
    public WorldMapSchema Map(WorldMap obj) => throw new NotImplementedException();
}