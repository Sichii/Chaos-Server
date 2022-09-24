using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public class SkillTemplateCache : SimpleFileCacheBase<SkillTemplate, SkillTemplateSchema, SkillTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<SkillTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public SkillTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<SkillTemplateCacheOptions> options,
        ILogger<SkillTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}