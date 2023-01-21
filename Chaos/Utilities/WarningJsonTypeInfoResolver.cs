using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Chaos.Serialization;
using Microsoft.Extensions.Logging;

namespace Chaos.Utilities;

public class WarningJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    private readonly ILogger<WarningJsonTypeInfoResolver> Logger;
    public WarningJsonTypeInfoResolver(ILogger<WarningJsonTypeInfoResolver> logger) => Logger = logger;

    /// <inheritdoc />
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        Logger.LogWarning(
            "Used reflection to get type info for {Type}. Add this type to {SerializationContext}",
            type.FullName,
            typeof(SerializationContext).FullName);

        return base.GetTypeInfo(type, options);
    }
}