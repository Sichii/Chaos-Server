using System.Text;
using System.Text.Json;
using Chaos.Collections.Common;
using Chaos.Common.Converters;
using Chaos.Testing.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class DynamicVarsConverterTests
{
    [Fact]
    public void Read_ShouldDeserializeDictionaryAndCreateDynamicVars()
    {
        // Arrange
        var converter = new DynamicVarsConverter();
        var options = new JsonSerializerOptions();
        const string JSON = "{\"key1\": 1, \"key2\": \"value2\"}";

        // Act
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(JSON));
        var result = converter.Read(ref reader, typeof(DynamicVars), options);

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(2);
        result.ContainsKey("key1").Should().BeTrue();
        result.ContainsKey("key2").Should().BeTrue();
        result.Get<int>("key1").Should().Be(1);
        result.Get<string>("key2").Should().Be("value2");
    }

    [Fact]
    public void Write_ShouldSerializeDynamicVarsAsDictionary()
    {
        // Arrange
        var converter = new DynamicVarsConverter();
        var options = new JsonSerializerOptions();

        var dic = new Dictionary<string, JsonElement>
        {
            ["key1"] = JsonDocument.Parse("1").RootElement,
            ["key2"] = JsonDocument.Parse("\"value2\"").RootElement
        };

        var vars = new DynamicVars(dic, new JsonSerializerOptions());

        const string EXPECTED_JSON = "{\"key1\":1,\"key2\":\"value2\"}";

        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);

        // Act
        converter.Write(writer, vars, options);

        var resultJson = Encoding.UTF8.GetString(memoryStream.ToArray());

        // Assert
        resultJson.Should().BeEquivalentToJson(EXPECTED_JSON);
    }
}