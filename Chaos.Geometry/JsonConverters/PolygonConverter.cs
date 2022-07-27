using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Geometry.Interfaces;

namespace Chaos.Geometry.JsonConverters;

public class PolygonConverter : JsonConverter<Polygon>
{
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
        }

        //read end of array
        reader.Read();

        return new Polygon(vertices);
    }

    public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var vertex in value)
            writer.WriteStringValue(IPoint.ToString(vertex));
        
        writer.WriteEndArray();
    }
}