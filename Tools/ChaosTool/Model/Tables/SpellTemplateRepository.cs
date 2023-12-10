using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class SpellTemplateRepository : RepositoryBase<SpellTemplateSchema>
{
    /// <inheritdoc />
    public SpellTemplateRepository(IEntityRepository entityRepository, IOptions<SpellTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}