using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.Storage.Abstractions;
using Chaos.Storage.Options;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Storage;

public sealed class MetafileCache : SimpleFileCacheBase<Metafile, MetafileSchema, MetafileCacheOptions>
{
    /// <inheritdoc />
    protected override Func<Metafile, string> KeySelector => m => m.Name;

    /// <inheritdoc />
    public MetafileCache(
        ITypeMapper mapper,
        IOptions<JsonSerializerOptions> jsonSerializerOptions,
        IOptionsSnapshot<MetafileCacheOptions> options,
        ILogger<MetafileCache> logger
    )
        : base(
            mapper,
            jsonSerializerOptions,
            options,
            logger) => AsyncHelpers.RunSync(ReloadAsync);
}