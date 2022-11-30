using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
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

public sealed class
    ExpiringSkillTemplateCache : ExpiringFileCacheBase<SkillTemplate, SkillTemplateSchema, ExpiringSkillTemplateCacheOptions>
{
    /// <inheritdoc />
    public ExpiringSkillTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringSkillTemplateCacheOptions> options,
        ILogger<ExpiringSkillTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}