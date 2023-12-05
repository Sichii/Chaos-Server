using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.JsonConverters;

/// <inheritdoc />
public sealed class PointConverter : JsonConverter<Point>
{
    /// <summary>
    ///     The singleton instance of <see cref="PointConverter" />
    /// </summary>
    public static JsonConverter<Point> Instance { get; } = new PointConverter();

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
    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
        => writer.WriteStringValue(IPoint.ToString(value));
}