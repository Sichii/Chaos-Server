using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Schemas.Templates;
using Chaos.Services.Storage.Options;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Storage;

public sealed class SkillTemplateCache : SimpleFileCacheBase<SkillTemplate, SkillTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<SkillTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public SkillTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<SkillTemplateCacheOptions> options,
        ILogger<SkillTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringSkillTemplateCache : ExpiringFileCacheBase<SkillTemplate, SkillTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringSkillTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringSkillTemplateCacheOptions> options,
        ILogger<ExpiringSkillTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}