using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using ChaosTool.Model.Abstractions;
using Microsoft.Extensions.Options;

namespace ChaosTool.Model.Tables;

public sealed class MerchantTemplateRepository : RepositoryBase<MerchantTemplateSchema>
{
    /// <inheritdoc />
    public MerchantTemplateRepository(IEntityRepository entityRepository, IOptions<MerchantTemplateCacheOptions> options)
        : base(entityRepository, options) { }
}