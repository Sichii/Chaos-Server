using System.Text;
using System.Text.Json;
using Chaos.Collections.Common;
using Chaos.Common.Converters;
using Chaos.Testing.Infrastructure.Definitions;
using Chaos.Testing.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class EnumCollectionConverterTests
{
    [Fact]
    public void Read_ShouldDeserializeDictionaryAndCreateEnumCollection()
    {
        // Arrange
        var converter = new EnumCollectionConverter();
        var options = new JsonSerializerOptions();
        const string JSON = "{\"SampleEnum1\": \"Value1\", \"SampleEnum2\": \"Value2\"}";

        // Act
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON));
        var result = converter.Read(ref reader, typeof(EnumCollection), options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .BeOfType<EnumCollection>();

        result.Count()
              .Should()
              .Be(2);

        result.TryGetValue<SampleEnum1>(out var enumType1Value)
              .Should()
              .BeTrue();

        enumType1Value.Should()
                      .Be(SampleEnum1.Value1);

        result.TryGetValue<SampleEnum2>(out var enumType2Value)
              .Should()
              .BeTrue();

        enumType2Value.Should()
                      .Be(SampleEnum2.Value2);
    }

    [Fact]
    public void Write_ShouldSerializeEnumCollectionAsDictionary()
    {
        // Arrange
        var converter = new EnumCollectionConverter();
        var options = new JsonSerializerOptions();
        var enumCollection = new EnumCollection();
        enumCollection.Set(SampleEnum2.Value2);
        enumCollection.Set(SampleEnum1.Value1);

        const string EXPECTED_JSON = "{\"SampleEnum1\":\"Value1\",\"SampleEnum2\":\"Value2\"}";

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);

        // Act
        converter.Write(writer, enumCollection, options);

        var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Assert
        resultJson.Should()
                  .BeEquivalentToJson(EXPECTED_JSON);
    }
}