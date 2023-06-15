using System.Text;
using System.Text.Json;
using Chaos.Geometry.JsonConverters;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class RectangleConverterTests
{
    [Fact]
    public void Read_ShouldReturnRectangle_WhenInputIsValid()
    {
        const string JSON_STRING = "{\"Top\": 1, \"Left\": 2, \"Width\": 3, \"Height\": 4}";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        var rectangle = RectangleConverter.Instance.Read(ref utf8JsonReader, typeof(Rectangle), null!);

        rectangle.Should()
                 .BeEquivalentTo(
                     new Rectangle(
                         2,
                         1,
                         3,
                         4));
    }

    [Fact]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsInvalid()
    {
        const string JSON_STRING = "{\"Left\": \"abcd\"}";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));
        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            RectangleConverter.Instance.Read(ref utf8JsonReader, typeof(Rectangle), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
    }

    [Fact]
    public void Write_ShouldWriteJson_WhenRectangleIsValid()
    {
        var rectangle = new Rectangle(
            2,
            1,
            3,
            4);

        var memoryStream = new MemoryStream();
        var utf8JsonWriter = new Utf8JsonWriter(memoryStream);

        RectangleConverter.Instance.Write(utf8JsonWriter, rectangle, null!);

        utf8JsonWriter.Flush();

        var jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());

        jsonString.Should().Be("{\"Top\":1,\"Left\":2,\"Width\":3,\"Height\":4}");
    }
}