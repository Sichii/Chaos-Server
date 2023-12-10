using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class ReactorTileTemplateRepository : RepositoryBase<ReactorTileTemplateSchema>
{
    /// <inheritdoc />
    public ReactorTileTemplateRepository(IEntityRepository entityRepository, IOptions<ReactorTileTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}