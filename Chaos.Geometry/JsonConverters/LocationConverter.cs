using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chaos.Geometry.JsonConverters;

public class LocationConverter : JsonConverter<Location>
{
    public override Location Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrEmpty(str))
            throw new InvalidOperationException("Expected a string");

        if (Location.TryParse(str, out var location))
            return location;

        throw new InvalidOperationException($"Invalid string format for location. \"{str}\"");
    }

    public override void Write(Utf8JsonWriter writer, Location value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}