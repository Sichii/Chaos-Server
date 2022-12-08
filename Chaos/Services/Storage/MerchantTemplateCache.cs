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

public sealed class MerchantTemplateCache : SimpleFileCacheBase<MerchantTemplate, MerchantTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<MerchantTemplate, string> KeySelector { get; } = m => m.TemplateKey;

    /// <inheritdoc />
    public MerchantTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<MerchantTemplateCacheOptions> options,
        ILogger<MerchantTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringMerchantTemplateCache : ExpiringFileCacheBase<MerchantTemplate, MerchantTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringMerchantTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringMerchantTemplateCacheOptions> options,
        ILogger<ExpiringMerchantTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}