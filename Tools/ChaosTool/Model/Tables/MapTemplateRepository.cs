using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class MapTemplateRepository : RepositoryBase<MapTemplateSchema>
{
    /// <inheritdoc />
    public MapTemplateRepository(IEntityRepository entityRepository, IOptions<MapTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}