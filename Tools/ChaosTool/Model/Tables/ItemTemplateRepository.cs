using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class ItemTemplateRepository : RepositoryBase<ItemTemplateSchema>
{
    /// <inheritdoc />
    public ItemTemplateRepository(IEntityRepository entityRepository, IOptions<ItemTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}