using System.Text.Json;
using Chaos.Core.Utilities;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Caches.Options;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Caches;

public class MonsterTemplateCache : SimpleFileCacheBase<MonsterTemplate, MonsterTemplateSchema, MonsterTemplateCacheOptions>
{
    /// <inheritdoc />
    public MonsterTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MonsterTemplateCacheOptions> options,
        ILogger<MonsterTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(LoadCacheAsync);

    /// <inheritdoc />
    protected override Func<MonsterTemplate, string> KeySelector => t => t.TemplateKey;
}