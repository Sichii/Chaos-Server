#region
using System.Text;
using System.Text.Json;
using Chaos.Geometry.JsonConverters;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class LocationConverterTests
{
    [Test]
    public void Read_ShouldReturnLocation_WhenInputIsValid()
    {
        const string JSON_STRING = "\"Example: (1, 2)\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        var location = LocationConverter.Instance.Read(ref utf8JsonReader, typeof(Location), null!);

        location.Should()
                .BeEquivalentTo(new Location("Example", 1, 2));
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
            LocationConverter.Instance.Read(ref utf8JsonReader, typeof(Location), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should()
          .NotBeNull();

        ex?.Message
          .Should()
          .Be("Expected a string");
    }

    [Test]
    public void Read_ShouldThrowInvalidOperationException_WhenInputIsInvalid()
    {
        const string JSON_STRING = "\"invalid\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON_STRING));

        utf8JsonReader.Read();

        InvalidOperationException? ex = null;

        try
        {
            LocationConverter.Instance.Read(ref utf8JsonReader, typeof(Location), null!);
        } catch (InvalidOperationException e)
        {
            ex = e;
        }

        ex.Should()
          .NotBeNull();

        ex?.Message
          .Should()
          .Be("Invalid string format for location. \"invalid\"");
    }

    [Test]
    public void Write_ShouldWriteJson_WhenLocationIsValid()
    {
        var location = new Location("Example", 1, 2);
        var memoryStream = new MemoryStream();
        var utf8JsonWriter = new Utf8JsonWriter(memoryStream);

        LocationConverter.Instance.Write(utf8JsonWriter, location, null!);

        utf8JsonWriter.Flush();

        var jsonString = Encoding.UTF8.GetString(memoryStream.ToArray());

        jsonString.Should()
                  .Be("\"Example:(1, 2)\"");
    }
}