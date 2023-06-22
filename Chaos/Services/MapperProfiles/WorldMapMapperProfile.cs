using Chaos.Collections;
using Chaos.Common.Identity;
using Chaos.Models.WorldMap;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class WorldMapMapperProfile : IMapperProfile<WorldMap, WorldMapSchema>,
                                     IMapperProfile<WorldMap, WorldMapArgs>,
                                     IMapperProfile<WorldMapNode, WorldMapNodeSchema>
{
    private static readonly SequentialIdGenerator<ushort> IdGenerator = new();
    private readonly ISimpleCache SimpleCache;

    public WorldMapMapperProfile(ISimpleCache simpleCache) => SimpleCache = simpleCache;

    /// <inheritdoc />
    public WorldMap Map(WorldMapArgs obj) => throw new NotImplementedException();

    /// <inheritdoc />
    WorldMapArgs IMapperProfile<WorldMap, WorldMapArgs>.Map(WorldMap obj) => new()
    {
        FieldName = obj.WorldMapKey,
        FieldIndex = obj.FieldIndex,
        Nodes = obj.Nodes.Values.Cast<WorldMapNodeInfo>().ToList()
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
        new(SimpleCache)
        {
            NodeKey = obj.NodeKey,
            UniqueId = IdGenerator.NextId,
            Destination = obj.Destination,
            ScreenPosition = obj.ScreenPosition,
            Text = obj.Text
        };

    /// <inheritdoc />
    public WorldMapNodeSchema Map(WorldMapNode obj) => throw new NotImplementedException();
}