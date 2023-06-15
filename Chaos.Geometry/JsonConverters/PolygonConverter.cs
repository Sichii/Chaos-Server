using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry.Abstractions;

namespace Chaos.Geometry.JsonConverters;

/// <inheritdoc />
public sealed class PolygonConverter : JsonConverter<Polygon>
{
    /// <summary>
    ///     The singleton instance of <see cref="PolygonConverter" />
    /// </summary>
    public static JsonConverter<Polygon> Instance { get; } = new PolygonConverter();

    /// <inheritdoc />
    public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new InvalidOperationException("Expected startArray");

        reader.Read();

        var vertices = new List<IPoint>();

        //read points till end of array
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            var str = reader.GetString()!;

            if (!Point.TryParse(str, out var point))
                throw new InvalidOperationException("Invalid point format");

            vertices.Add(point);
            reader.Read();
        }

        //read end of array
        reader.Read();

        return new Polygon(vertices);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var vertex in value)
            writer.WriteStringValue(IPoint.ToString(vertex));

        writer.WriteEndArray();
    }
}