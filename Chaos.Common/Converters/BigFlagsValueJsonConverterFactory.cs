#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.CustomTypes;
#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.Converters;

/// <summary>
///     JSON converter factory for BigFlagsValue that creates the appropriate converter for any TMarker type. This allows
///     [JsonConverter(typeof(BigFlagsValueJsonConverterFactory))] to work on BigFlagsValue.
/// </summary>
public sealed class BigFlagsValueJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        var genericType = typeToConvert.GetGenericTypeDefinition();

        return genericType == typeof(BigFlagsValue<>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var markerType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(BigFlagsValueJsonConverter<>).MakeGenericType(markerType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}