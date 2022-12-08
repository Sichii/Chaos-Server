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

public sealed class MonsterTemplateCache : SimpleFileCacheBase<MonsterTemplate, MonsterTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<MonsterTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public MonsterTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<MonsterTemplateCacheOptions> options,
        ILogger<MonsterTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringMonsterTemplateCache : ExpiringFileCacheBase<MonsterTemplate, MonsterTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringMonsterTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringMonsterTemplateCacheOptions> options,
        ILogger<ExpiringMonsterTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}