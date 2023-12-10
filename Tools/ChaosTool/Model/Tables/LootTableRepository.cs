using Chaos.Schemas.Content;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class LootTableRepository : RepositoryBase<LootTableSchema>
{
    /// <inheritdoc />
    public LootTableRepository(IEntityRepository entityRepository, IOptions<LootTableCacheOptions> options)
        : base(entityRepository, options) { }
}