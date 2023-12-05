using System.Text;
using System.Text.Json;
using Chaos.Collections.Common;
using Chaos.Common.Converters;
using Chaos.Testing.Infrastructure.Definitions;
using Chaos.Testing.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

// ReSharper disable UnusedMember.Local

namespace Chaos.Common.Tests;

public sealed class FlagCollectionConverterTests
{
    [Fact]
    public void Read_ShouldDeserializeJsonObjectAndCreateFlagCollection()
    {
        // Arrange
        var converter = new FlagCollectionConverter();
        var options = new JsonSerializerOptions();
        const string JSON = "{\"ColorFlag\":\"Red\",\"SizeFlag\":\"Small\"}";

        // Act
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON));
        var result = converter.Read(ref reader, typeof(FlagCollection), options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .BeOfType<FlagCollection>();

        result.HasFlag(ColorFlag.Red)
              .Should()
              .BeTrue();

        result.HasFlag(SizeFlag.Small)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void Write_ShouldSerializeFlagCollectionAsJsonObject()
    {
        // Arrange
        var converter = new FlagCollectionConverter();
        var options = new JsonSerializerOptions();
        var flagCollection = new FlagCollection();
        flagCollection.AddFlag(ColorFlag.Red);
        flagCollection.AddFlag(SizeFlag.Small);
        const string EXPECTED_JSON = "{\"ColorFlag\":\"Red\",\"SizeFlag\":\"Small\"}";

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);

        // Act
        converter.Write(writer, flagCollection, options);

        var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Assert
        resultJson.Should()
                  .BeEquivalentToJson(EXPECTED_JSON);
    }
}