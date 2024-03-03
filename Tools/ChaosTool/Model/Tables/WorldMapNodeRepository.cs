using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public class WorldMapNodeRepository : RepositoryBase<WorldMapNodeSchema>
{
    public WorldMapNodeRepository(IEntityRepository entityRepository, IOptions<WorldMapNodeCacheOptions>? options)
        : base(entityRepository, options) { }
}