using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chaos.Geometry.JsonConverters;

public class RectangleConverter : JsonConverter<Rectangle>
{
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new InvalidOperationException("Expected startObject");

        //read startobj
        reader.Read();

        var top = 0;
        var left = 0;
        var width = 0;
        var height = 0;
        
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            var pName = reader.GetString()!.ToLower();
            reader.Read(); //read separator
            var value = reader.GetInt32();

            switch (pName)
            {
                case "top":
                    top = value;

                    break;
                case "left":
                    left = value;

                    break;
                case "width":
                    width = value;

                    break;
                case "height":
                    height = value;

                    break;
            }
        }

        reader.Read();

        return new Rectangle(
            top,
            left,
            width,
            height);
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName("Top");
        
        writer.WriteNumber("Top", value.Top);
        writer.WriteNumber("Left", value.Left);
        writer.WriteNumber("Width", value.Width);
        writer.WriteNumber("Height", value.Height);

        writer.WriteEndObject();
    }
}