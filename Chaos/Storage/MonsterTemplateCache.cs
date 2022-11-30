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

public sealed class MonsterTemplateCache : SimpleFileCacheBase<MonsterTemplate, MonsterTemplateSchema, MonsterTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<MonsterTemplate, string> KeySelector => t => t.TemplateKey;

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
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringMonsterTemplateCache : ExpiringFileCacheBase<MonsterTemplate, MonsterTemplateSchema, ExpiringMonsterTemplateCacheOptions>
{
    /// <inheritdoc />
    public ExpiringMonsterTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringMonsterTemplateCacheOptions> options,
        ILogger<ExpiringMonsterTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}