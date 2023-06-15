using System.Text;
using System.Text.Json;
using Chaos.Geometry.JsonConverters;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class PointConverterTests
{
    [Fact]
    public void Read_ShouldReturnPoint_WhenInputIsValid()
    {
        const string JSON_STRING = "\"(1, 2)\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        var point = PointConverter.Instance.Read(ref utf8JsonReader, typeof(Point), null!);

        point.Should().BeEquivalentTo(new Point(1, 2));
    }

    [Fact]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsEmpty()
    {
        const string JSON_STRING = "\"\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            PointConverter.Instance.Read(ref utf8JsonReader, typeof(Point), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex?.Message.Should().Be("Expected a string");
    }

    [Fact]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsInvalid()
    {
        var jsonString = "\"invalid\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonString));

        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            PointConverter.Instance.Read(ref utf8JsonReader, typeof(Point), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should().NotBeNull();
        ex?.Message.Should().Be("Invalid string format for point. \"invalid\"");
    }

    [Fact]
    public void Write_ShouldWriteJson_WhenPointIsValid()
    {
        var point = new Point(1, 2);
        var memoryStream = new MemoryStream();
        var utf8JsonWriter = new Utf8JsonWriter(memoryStream);

        PointConverter.Instance.Write(utf8JsonWriter, point, null!);

        utf8JsonWriter.Flush();

        var jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());

        jsonString.Should().Be("\"(1, 2)\"");
    }
}