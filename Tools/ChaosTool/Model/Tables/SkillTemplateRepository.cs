using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class SkillTemplateRepository : RepositoryBase<SkillTemplateSchema>
{
    /// <inheritdoc />
    public SkillTemplateRepository(IEntityRepository entityRepository, IOptions<SkillTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}