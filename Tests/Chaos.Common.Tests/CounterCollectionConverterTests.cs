using System.Text;
using System.Text.Json;
using Chaos.Collections.Common;
using Chaos.Common.Converters;
using Chaos.Testing.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class CounterCollectionConverterTests
{
    [Fact]
    public void Read_ShouldDeserializeDictionaryAndCreateCounterCollection()
    {
        // Arrange
        var converter = new CounterCollectionConverter();
        var options = new JsonSerializerOptions();
        const string JSON = "{\"counter1\": 1, \"counter2\": 2}";

        // Act
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON));
        var result = converter.Read(ref reader, typeof(CounterCollection), options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Count()
              .Should()
              .Be(2);

        result.TryGetValue("counter1", out var counter1)
              .Should()
              .BeTrue();

        result.TryGetValue("counter2", out var counter2)
              .Should()
              .BeTrue();

        counter1.Should()
                .Be(1);

        counter2.Should()
                .Be(2);
    }

    [Fact]
    public void Write_ShouldSerializeCounterCollectionAsDictionary()
    {
        // Arrange
        var converter = new CounterCollectionConverter();
        var options = new JsonSerializerOptions();
        var collection = new CounterCollection();
        collection.AddOrIncrement("counter1");
        collection.AddOrIncrement("counter2", 2);

        const string EXPECTED_JSON = "{\"counter1\":1,\"counter2\":2}";

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);

        // Act
        converter.Write(writer, collection, options);
        var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Assert
        resultJson.Should()
                  .BeEquivalentToJson(EXPECTED_JSON);
    }
}