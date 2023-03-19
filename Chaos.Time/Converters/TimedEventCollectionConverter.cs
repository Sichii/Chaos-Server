using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Time;

namespace Chaos.Time.Converters;

/// <summary>
///     A converter for <see cref="TimedEventCollection" />
/// </summary>
public sealed class TimedEventCollectionConverter : JsonConverter<TimedEventCollection>
{
    /// <inheritdoc />
    public override TimedEventCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var serializedDictionary = JsonSerializer.Deserialize<Dictionary<string, TimedEventCollection.Event>>(ref reader, options);

        return new TimedEventCollection(serializedDictionary?.Values);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimedEventCollection value, JsonSerializerOptions options)
    {
        var serializedDictionary = new Dictionary<string, TimedEventCollection.Event>(value);

        JsonSerializer.Serialize(writer, serializedDictionary, options);
    }
}