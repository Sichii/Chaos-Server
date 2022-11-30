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

public sealed class MerchantTemplateCache : SimpleFileCacheBase<MerchantTemplate, MerchantTemplateSchema, MerchantTemplateCacheOptions>
{
    /// <inheritdoc />
    protected override Func<MerchantTemplate, string> KeySelector { get; } = m => m.TemplateKey;

    /// <inheritdoc />
    public MerchantTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MerchantTemplateCacheOptions> options,
        ILogger<MerchantTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringMerchantTemplateCache : ExpiringFileCacheBase<MerchantTemplate, MerchantTemplateSchema, ExpiringMerchantTemplateCacheOptions>
{
    /// <inheritdoc />
    public ExpiringMerchantTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<ExpiringMerchantTemplateCacheOptions> options,
        ILogger<ExpiringMerchantTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}