using Chaos.Collections;
using Chaos.Models.WorldMap;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class WorldMapMapperProfile : IMapperProfile<WorldMap, WorldMapSchema>,
                                     IMapperProfile<WorldMap, WorldMapArgs>,
                                     IMapperProfile<WorldMapNode, WorldMapNodeSchema>,
                                     IMapperProfile<WorldMapNode, WorldMapNodeInfo>
{
    private readonly ITypeMapper Mapper;
    private readonly ISimpleCache SimpleCache;

    public WorldMapMapperProfile(ISimpleCache simpleCache, ITypeMapper mapper)
    {
        SimpleCache = simpleCache;
        Mapper = mapper;
    }

    /// <inheritdoc />
    public WorldMap Map(WorldMapArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    WorldMapArgs IMapperProfile<WorldMap, WorldMapArgs>.Map(WorldMap obj) => new()
    {
        FieldName = obj.WorldMapKey,
        FieldIndex = obj.FieldIndex,
        Nodes = Mapper.MapMany<WorldMapNodeInfo>(obj.Nodes.Values).ToList()
    };

    /// <inheritdoc />
    public WorldMap Map(WorldMapSchema obj) => new()
    {
        WorldMapKey = obj.WorldMapKey,
        FieldIndex = obj.FieldIndex,
        Nodes = obj.NodeKeys.Select(key => SimpleCache.Get<WorldMapNode>(key))
                   .ToDictionary(node => node.UniqueId)
    };

    /// <inheritdoc />
    public WorldMapSchema Map(WorldMap obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public WorldMapNode Map(WorldMapNodeSchema obj) =>
        new(
            SimpleCache,
            obj.NodeKey,
            obj.Destination,
            obj.ScreenPosition,
            obj.Text);

    /// <inheritdoc />
    public WorldMapNode Map(WorldMapNodeInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    WorldMapNodeInfo IMapperProfile<WorldMapNode, WorldMapNodeInfo>.Map(WorldMapNode obj) => new()
    {
        CheckSum = obj.UniqueId, //check for this node id on the current world map, use the destination from there, not here
        DestinationPoint = new Point(), //value not trusted, dont bother populating 
        MapId = 0, //value not trusted, dont bother populating
        ScreenPosition = obj.ScreenPosition,
        Text = obj.Text
    };

    /// <inheritdoc />
    public WorldMapNodeSchema Map(WorldMapNode obj) => throw new NotImplementedException();
}