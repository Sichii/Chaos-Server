using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;

namespace Chaos.Common.Converters;

/// <summary>
///     A converter for <see cref="CounterCollection" />
/// </summary>
public sealed class CounterCollectionConverter : JsonConverter<CounterCollection>
{
    /// <inheritdoc />
    public override CounterCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var serializedDictionary = JsonSerializer.Deserialize<Dictionary<string, int>>(ref reader, options);

        return new CounterCollection(serializedDictionary);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, CounterCollection value, JsonSerializerOptions options)
    {
        var serializedDictionary = new Dictionary<string, int>(value);

        JsonSerializer.Serialize(writer, serializedDictionary, options);
    }
}