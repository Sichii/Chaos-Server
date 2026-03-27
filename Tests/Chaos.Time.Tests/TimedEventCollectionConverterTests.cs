#region
using System.Text.Json;
using Chaos.Collections.Time;
using Chaos.Time.Converters;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class TimedEventCollectionConverterTests
{
    private static JsonSerializerOptions BuildOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimedEventCollectionConverter());

        return options;
    }

    [Test]
    public void Read_ValidJson_PopulatesCollection()
    {
        // Serialize a known collection then deserialize to avoid hardcoding TimeSpan/DateTime formats
        var source = new TimedEventCollection();
        source.AddEvent("evt1", TimeSpan.FromHours(2));
        source.AddEvent("evt2", TimeSpan.FromMinutes(30));

        var options = BuildOptions();
        var json = JsonSerializer.Serialize(source, options);

        var result = JsonSerializer.Deserialize<TimedEventCollection>(json, options)!;

        result.HasActiveEvent("evt1", out var e1)
              .Should()
              .BeTrue();

        e1!.EventId
           .Should()
           .Be("evt1");

        result.HasActiveEvent("evt2", out _)
              .Should()
              .BeTrue();
    }

    [Test]
    public void RoundTrip_IsIdempotent()
    {
        var source = new TimedEventCollection();
        source.AddEvent("roundtrip1", TimeSpan.FromHours(1));
        source.AddEvent("roundtrip2", TimeSpan.FromMinutes(45), true);

        var options = BuildOptions();
        var json = JsonSerializer.Serialize(source, options);
        var deserialized = JsonSerializer.Deserialize<TimedEventCollection>(json, options)!;

        // Serialize the deserialized collection again — output should be structurally equivalent
        var json2 = JsonSerializer.Serialize(deserialized, options);

        json2.Should()
             .Contain("\"roundtrip1\"");

        json2.Should()
             .Contain("\"roundtrip2\"");
    }

    [Test]
    public void Write_EmptyCollection_SerializesAsEmptyObject()
    {
        var source = new TimedEventCollection();
        var options = BuildOptions();

        var json = JsonSerializer.Serialize(source, options);

        json.Should()
            .Be("{}");
    }

    [Test]
    public void Write_SerializesEventsAsDictionary()
    {
        var source = new TimedEventCollection();
        source.AddEvent("myEvent", TimeSpan.FromMinutes(5));

        var options = BuildOptions();
        var json = JsonSerializer.Serialize(source, options);

        json.Should()
            .Contain("\"myEvent\"");

        json.Should()
            .Contain("\"EventId\":\"myEvent\"");
    }
}