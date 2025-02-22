#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Specialized;
#endregion

namespace Chaos.Collections.Converters;

/// <inheritdoc />
[JsonConverter(typeof(FixedSetJsonConverter<>))]
public sealed class FixedSetJsonConverter<T> : JsonConverter<FixedSet<T>> where T: notnull
{
    /// <inheritdoc />
    public override FixedSet<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // We expect: { "Capacity": X, "Items": [ ... ] }
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Expected start of object to deserialize {typeof(FixedSet<T>).Name}.");

        int? capacity = null;
        List<T> items = [];

        // Read the object
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propName = reader.GetString()!;

                // Move to the property value
                if (!reader.Read())
                    throw new JsonException("Incomplete JSON.");

                if (propName.Equals("Capacity", StringComparison.OrdinalIgnoreCase))
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new JsonException("Property 'Capacity' must be an integer.");

                    capacity = reader.GetInt32();
                } else if (propName.Equals("Items", StringComparison.OrdinalIgnoreCase))
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException("Property 'Items' must be a JSON array.");

                    // Read each element in the array
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            break;

                        // Deserialize each item using the default serializer for T
                        var item = JsonSerializer.Deserialize<T>(ref reader, options);

                        if (item is not null)
                            items.Add(item);
                    }
                } else

                    // Unknown property -> skip
                    reader.Skip();
            }
        }

        if (capacity is null)
            throw new JsonException("Missing 'Capacity' property for FixedSet<T> JSON.");

        // Construct a new FixedSet<T> with capacity
        var set = new FixedSet<T>(capacity.Value, items);

        return set;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, FixedSet<T> value, JsonSerializerOptions options)
    {
        // We write: { "Capacity": X, "Items": [ ... ] }
        writer.WriteStartObject();

        // Write the capacity
        writer.WriteNumber("Capacity", value.Capacity);

        // Write the items array
        writer.WritePropertyName("Items");
        writer.WriteStartArray();

        // The enumerator goes oldest -> newest in your class
        foreach (var item in value)
            JsonSerializer.Serialize(writer, item, options);

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}