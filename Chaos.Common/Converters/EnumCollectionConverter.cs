#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="EnumCollection" />
/// </summary>
public sealed class EnumCollectionConverter : JsonConverter<EnumCollection>
{
    /// <inheritdoc />
    public override EnumCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var serializedDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);

        var enumCollection = new EnumCollection();

        foreach (var kvp in serializedDictionary!)
        {
            var enumType = TypeCache.GetEnumType(kvp.Key) ?? throw new JsonException($"Could not resolve enum type: {kvp.Key}");

            var enumValue = (Enum)Enum.Parse(enumType, kvp.Value);
            enumCollection.Set(enumType, enumValue);
        }

        return enumCollection;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, EnumCollection value, JsonSerializerOptions options)
    {
        var serializedDictionary = new Dictionary<string, string>();

        foreach (var kvp in value)
        {
            var flagName = kvp.Key.Name;

            var flagValue = Enum.ToObject(kvp.Key, kvp.Value)
                                .ToString();

            serializedDictionary.Add(flagName, flagValue!);
        }

        JsonSerializer.Serialize(writer, serializedDictionary, options);
    }
}