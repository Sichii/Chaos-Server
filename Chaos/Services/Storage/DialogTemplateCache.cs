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

public sealed class DialogTemplateCache : SimpleFileCacheBase<DialogTemplate, DialogTemplateSchema>
{
    /// <inheritdoc />
    protected override Func<DialogTemplate, string> KeySelector { get; } = d => d.TemplateKey;

    /// <inheritdoc />
    public DialogTemplateCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<DialogTemplateCacheOptions> options,
        ILogger<DialogTemplateCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions.Value,
            options.Value,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}

public sealed class
    ExpiringDialogTemplateCache : ExpiringFileCacheBase<DialogTemplate, DialogTemplateSchema>
{
    /// <inheritdoc />
    public ExpiringDialogTemplateCache(
        IMemoryCache cache,
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptions<ExpiringDialogTemplateCacheOptions> options,
        ILogger<ExpiringDialogTemplateCache> logger
    )
        : base(
            cache,
            mapper,
            jsonSerializerOptions,
            options,
            logger) { }
}