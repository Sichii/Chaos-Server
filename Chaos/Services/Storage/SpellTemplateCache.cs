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

public sealed class SpellTemplateCache : SimpleFileCacheBase<SpellTemplate, SpellTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<SpellTemplate, string> KeySelector => t => t.TemplateKey;

    /// <inheritdoc />
    public SpellTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<SpellTemplateCacheOptions> options,
        ILogger<SpellTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringSpellTemplateCache : ExpiringFileCacheBase<SpellTemplate, SpellTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringSpellTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringSpellTemplateCacheOptions> options,
        ILogger<ExpiringSpellTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}