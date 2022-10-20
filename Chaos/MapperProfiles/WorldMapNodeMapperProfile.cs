using Chaos.Common.Identity;
using Chaos.Data;
using Chaos.Schemas.Data;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class WorldMapNodeMapperProfile : IMapperProfile<WorldMapNode, WorldMapNodeSchema>
{
    private static readonly IdGenerator<ushort> IdGenerator = new();
    private readonly ISimpleCache SimpleCache;
    public WorldMapNodeMapperProfile(ISimpleCache simpleCache) => SimpleCache = simpleCache;

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