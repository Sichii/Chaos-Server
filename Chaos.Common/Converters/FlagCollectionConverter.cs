#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="FlagCollection" />
/// </summary>
public sealed class FlagCollectionConverter : JsonConverter<FlagCollection>
{
    /// <inheritdoc />
    public override FlagCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var serializedDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);

        var flagCollection = new FlagCollection();

        foreach (var kvp in serializedDictionary!)
        {
            var flagType = TypeCache.GetEnumType(kvp.Key) ?? throw new JsonException($"Could not resolve enum type: {kvp.Key}");

            var flagValue = (Enum)Enum.Parse(flagType, kvp.Value);
            flagCollection.AddFlag(flagType, flagValue);
        }

        return flagCollection;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FlagCollection value, JsonSerializerOptions options)
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