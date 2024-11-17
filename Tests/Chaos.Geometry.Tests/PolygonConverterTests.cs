#region
using System.Text;
using System.Text.Json;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.JsonConverters;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class PolygonConverterTests
{
    [Test]
    public void Read_ShouldReturnPolygon_WhenInputIsValid()
    {
        const string JSON_STRING = "[\"(1, 2)\", \"(3, 4)\", \"(5, 6)\"]";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        var polygon = PolygonConverter.Instance.Read(ref utf8JsonReader, typeof(Polygon), null!);

        var expectedPolygon = new Polygon(
            new List<IPoint>
            {
                new Point(1, 2),
                new Point(3, 4),
                new Point(5, 6)
            });

        polygon.Should()
               .BeEquivalentTo(expectedPolygon);
    }

    [Test]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsEmpty()
    {
        const string JSON_STRING = "\"\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            PolygonConverter.Instance.Read(ref utf8JsonReader, typeof(Polygon), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should()
          .NotBeNull();

        ex?.Message
          .Should()
          .Be("Expected startArray");
    }

    [Test]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsInvalid()
    {
        const string JSON_STRING = "[\"invalid\", \"(1, 2)\"]";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            PolygonConverter.Instance.Read(ref utf8JsonReader, typeof(Polygon), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should()
          .NotBeNull();

        ex?.Message
          .Should()
          .Be("Invalid point format");
    }

    [Test]
    public void Write_ShouldWriteJson_WhenPolygonIsValid()
    {
        var polygon = new Polygon(
            new List<IPoint>
            {
                new Point(1, 2),
                new Point(3, 4),
                new Point(5, 6)
            });
        var memoryStream = new MemoryStream();
        var utf8JsonWriter = new Utf8JsonWriter(memoryStream);

        PolygonConverter.Instance.Write(utf8JsonWriter, polygon, null!);

        utf8JsonWriter.Flush();

        var jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());

        jsonString.Should()
                  .Be("[\"(1, 2)\",\"(3, 4)\",\"(5, 6)\"]");
    }
}