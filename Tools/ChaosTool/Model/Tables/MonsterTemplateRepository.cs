using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class MonsterTemplateRepository : RepositoryBase<MonsterTemplateSchema>
{
    /// <inheritdoc />
    public MonsterTemplateRepository(IEntityRepository entityRepository, IOptions<MonsterTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}