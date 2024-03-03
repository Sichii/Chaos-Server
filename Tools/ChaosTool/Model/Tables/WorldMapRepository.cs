using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public class WorldMapRepository : RepositoryBase<WorldMapSchema>
{
    public WorldMapRepository(IEntityRepository entityRepository, IOptions<WorldMapCacheOptions>? options)
        : base(entityRepository, options) { }
}