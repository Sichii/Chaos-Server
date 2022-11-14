using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class SkillTemplateCache : SimpleFileCacheBase<SkillTemplate, SkillTemplateSchema, SkillTemplateCacheOptions>
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