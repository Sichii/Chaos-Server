#region
using System.Text;
using System.Text.Json;
using Chaos.Common.Converters;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class EnumerableConverterTests
{
    [Test]
    public void Read_ShouldDeserializeJsonArrayAndCreateEnumerableInstance()
    {
        // Arrange
        var converter = new EnumerableConverter<List<int>, int>();
        var options = new JsonSerializerOptions();
        const string JSON = "[1, 2, 3]";

        // Act
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON));
        var result = converter.Read(ref reader, typeof(List<int>), options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .BeOfType<List<int>>();

        result.Count
              .Should()
              .Be(3);

        result.Should()
              .BeEquivalentTo(
                  new List<int>
                  {
                      1,
                      2,
                      3
                  });
    }

    [Test]
    public void Write_ShouldSerializeEnumerableInstanceAsJsonArray()
    {
        // Arrange
        var converter = new EnumerableConverter<List<int>, int>();
        var options = new JsonSerializerOptions();

        var enumerable = new List<int>
        {
            1,
            2,
            3
        };
        const string EXPECTED_JSON = "[1,2,3]";

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);

        // Act
        converter.Write(writer, enumerable, options);

        var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Assert
        resultJson.Should()
                  .Be(EXPECTED_JSON);
    }
}