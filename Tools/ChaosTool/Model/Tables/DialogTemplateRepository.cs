using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class DialogTemplateRepository : RepositoryBase<DialogTemplateSchema>
{
    /// <inheritdoc />
    public DialogTemplateRepository(IEntityRepository entityRepository, IOptions<DialogTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}