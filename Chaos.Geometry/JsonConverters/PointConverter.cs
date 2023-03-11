using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.JsonConverters;

/// <inheritdoc />
public sealed class PointConverter : JsonConverter<Point>
{
    /// <inheritdoc />
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrEmpty(str))
            throw new InvalidOperationException("Expected a string");

        if (Point.TryParse(str, out var point))
            return point;

        throw new InvalidOperationException($"Invalid string format for point. \"{str}\"");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options) =>
        writer.WriteStringValue(IPoint.ToString(value));
}