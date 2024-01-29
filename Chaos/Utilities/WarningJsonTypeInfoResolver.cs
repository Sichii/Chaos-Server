using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Chaos.Extensions.Common;

namespace Chaos.Utilities;

public sealed class WarningJsonTypeInfoResolver(ILogger<WarningJsonTypeInfoResolver> logger) : DefaultJsonTypeInfoResolver
{
    private readonly ILogger<WarningJsonTypeInfoResolver> Logger = logger;

    /// <inheritdoc />
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (!type.IsCompilerGenerated() && type is { IsInterface: false, IsAbstract: false, IsGenericTypeDefinition: false })
            Logger.LogTrace(
                "Used reflection to get type info for type {@Type}. Add this type to the {@SerializationContext}",
                type.FullName,
                typeof(SerializationContext).FullName);

        return base.GetTypeInfo(type, options);
    }
}