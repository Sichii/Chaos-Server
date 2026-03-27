#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.CustomTypes;
#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.Converters;

/// <summary>
///     JSON converter for BigFlagsValue that serializes to/from comma-delimited flag names. Example: "Feature1, Feature2,
///     Feature3"
/// </summary>
public sealed class BigFlagsValueJsonConverter<TMarker> : JsonConverter<BigFlagsValue<TMarker>> where TMarker: BigFlags<TMarker>
{
    /// <inheritdoc />
    public override BigFlagsValue<TMarker> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return BigFlags<TMarker>.None;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Expected string token for BigFlagsValue<{typeof(TMarker).Name}>, got {reader.TokenType}");

        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value) || value.Equals("None", StringComparison.OrdinalIgnoreCase))
            return BigFlags<TMarker>.None;

        if (BigFlags<TMarker>.TryParse(value, true, out var result))
            return result;

        throw new JsonException($"Unknown flag name '{value}' for type {typeof(TMarker).Name}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BigFlagsValue<TMarker> value, JsonSerializerOptions options)
    {
        var str = BigFlags<TMarker>.ToString(value);
        writer.WriteStringValue(str);
    }
}