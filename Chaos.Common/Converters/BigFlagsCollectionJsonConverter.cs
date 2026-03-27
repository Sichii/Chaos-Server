#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Common.CustomTypes;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     JSON converter for BigFlagsCollection that serializes to a dictionary of type names to comma-delimited flag names
/// </summary>
public sealed class BigFlagsCollectionJsonConverter : JsonConverter<BigFlagsCollection>
{
    /// <inheritdoc />
    public override BigFlagsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var collection = new BigFlagsCollection();

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return collection;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name");

            var typeName = reader.GetString();

            if (string.IsNullOrEmpty(typeName))
                throw new JsonException("Type name cannot be null or empty");

            var markerType = TypeCache.GetType(typeName) ?? throw new JsonException($"Could not resolve type: {typeName}");

            // Read the value
            reader.Read();

            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string value for flags");

            var flagsString = reader.GetString();

            if (string.IsNullOrEmpty(flagsString) || flagsString.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                collection.AddFlag(markerType, BigFlags.GetNone(markerType));

                continue;
            }

            if (BigFlags.TryParse(
                    markerType,
                    flagsString,
                    true,
                    out var flagValue))
                collection.AddFlag(markerType, flagValue);
            else
                throw new JsonException($"Unknown flag name '{flagsString}' for type {markerType.Name}");
        }

        throw new JsonException("Unexpected end of JSON");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BigFlagsCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach ((var markerType, var flagValue) in value)
        {
            var typeName = markerType.Name;
            var flagsString = BigFlags.ToString(markerType, flagValue);

            writer.WriteString(typeName, flagsString);
        }

        writer.WriteEndObject();
    }
}