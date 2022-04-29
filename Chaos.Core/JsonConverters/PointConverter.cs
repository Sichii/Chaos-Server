using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Core.Geometry;

namespace Chaos.Core.JsonConverters;

public class PointConverter : JsonConverter<Point>
{
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrEmpty(str))
            throw new InvalidOperationException("Expected a string");

        if (Point.TryParse(str, out var point))
            return point;

        throw new InvalidOperationException($"Invalid string format for point. \"{str}\"");
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}