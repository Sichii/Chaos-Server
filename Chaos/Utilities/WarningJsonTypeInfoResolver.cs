using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Chaos.Extensions.Common;
using Microsoft.Extensions.Logging;

namespace Chaos.Utilities;

public sealed class WarningJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    private readonly ILogger<WarningJsonTypeInfoResolver> Logger;
    public WarningJsonTypeInfoResolver(ILogger<WarningJsonTypeInfoResolver> logger) => Logger = logger;

    /// <inheritdoc />
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (!type.IsCompilerGenerated())
            Logger.LogTrace(
                "Used reflection to get type info for type {@Type}. Add this type to the {@SerializationContext}",
                type.FullName,
                typeof(SerializationContext).FullName);

        return base.GetTypeInfo(type, options);
    }
}