#region
using System.Text.Json;
using Chaos.Collections.Converters;
using Chaos.Collections.Specialized;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class FixedSetJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public FixedSetJsonConverterTests()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new FixedSetJsonConverter<string>());
        _options.Converters.Add(new FixedSetJsonConverter<int>());
        _options.Converters.Add(new FixedSetJsonConverter<TestItem>());
    }

    [Test]
    public void Read_CapacityNotANumber_ThrowsJsonException()
    {
        // Arrange
        var json = "{\"Capacity\":\"notanumber\",\"Items\":[\"item1\"]}";

        // Act
        var act = () => JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("Property 'Capacity' must be an integer.");
    }

    [Test]
    public void Read_CaseInsensitivePropertyNames_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"capacity\":3,\"items\":[\"item1\",\"item2\"]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(3);

        result.Count
              .Should()
              .Be(2);

        result.Should()
              .Contain("item1");

        result.Should()
              .Contain("item2");
    }

    [Test]
    public void Read_ComplexObjects_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"Capacity\":2,\"Items\":[{\"Id\":1,\"Name\":\"First\"},{\"Id\":2,\"Name\":\"Second\"}]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<TestItem>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(2);

        result.Count
              .Should()
              .Be(2);

        var items = result.ToList();

        items[0]
            .Id
            .Should()
            .Be(1);

        items[0]
            .Name
            .Should()
            .Be("First");

        items[1]
            .Id
            .Should()
            .Be(2);

        items[1]
            .Name
            .Should()
            .Be("Second");
    }

    [Test]
    public void Read_EmptyItemsArray_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"Capacity\":5,\"Items\":[]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(5);

        result.Count
              .Should()
              .Be(0);
    }

    [Test]
    public void Read_ExtraUnknownProperties_IgnoresThemAndDeserializesCorrectly()
    {
        // Arrange
        var json = "{\"Capacity\":2,\"Items\":[\"item1\"],\"UnknownProperty\":\"value\",\"AnotherUnknown\":123}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(2);

        result.Count
              .Should()
              .Be(1);

        result.Should()
              .Contain("item1");
    }

    [Test]
    public void Read_IncompleteJson_ThrowsJsonException()
    {
        // Arrange
        var json = "{\"Capacity\":"; // Incomplete JSON

        // Act
        var act = () => JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        act.Should()
           .Throw<JsonException>();
    }

    [Test]
    public void Read_ItemsNotAnArray_ThrowsJsonException()
    {
        // Arrange
        var json = "{\"Capacity\":3,\"Items\":\"notanarray\"}";

        // Act
        var act = () => JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("Property 'Items' must be a JSON array.");
    }

    [Test]
    public void Read_ItemsWithNullValues_SkipsNullItems()
    {
        // Arrange - Note: This test assumes the FixedSet can handle the scenario where some items are null during deserialization
        var json = "{\"Capacity\":3,\"Items\":[\"item1\",null,\"item2\"]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(3);

        result.Count
              .Should()
              .Be(2); // null item should be skipped

        result.Should()
              .Contain("item1");

        result.Should()
              .Contain("item2");
    }

    [Test]
    public void Read_MissingCapacityProperty_ThrowsJsonException()
    {
        // Arrange
        var json = "{\"Items\":[\"item1\",\"item2\"]}";

        // Act
        var act = () => JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("Missing 'Capacity' property for FixedSet<T> JSON.");
    }

    [Test]
    public void Read_MixedCasePropertyNames_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"CaPaCiTy\":4,\"ItEmS\":[\"test\"]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(4);

        result.Count
              .Should()
              .Be(1);

        result.Should()
              .Contain("test");
    }

    [Test]
    public void Read_NotStartingWithObject_ThrowsJsonException()
    {
        // Arrange
        var json = "[\"item1\",\"item2\"]"; // Array instead of object

        // Act
        var act = () => JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("Expected start of object to deserialize FixedSet`1.");
    }

    [Test]
    public void Read_OnlyCapacityProperty_CreatesEmptyFixedSet()
    {
        // Arrange
        var json = "{\"Capacity\":10}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(10);

        result.Count
              .Should()
              .Be(0);
    }

    [Test]
    public void Read_PropertiesInDifferentOrder_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"Items\":[\"item1\",\"item2\"],\"Capacity\":5}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(5);

        result.Count
              .Should()
              .Be(2);

        result.Should()
              .Contain("item1");

        result.Should()
              .Contain("item2");
    }

    [Test]
    public void Read_ValidJson_DeserializesCorrectly()
    {
        // Arrange
        var json = "{\"Capacity\":3,\"Items\":[\"item1\",\"item2\",\"item3\"]}";

        // Act
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(3);

        result.Count
              .Should()
              .Be(3);

        result.Should()
              .Contain("item1");

        result.Should()
              .Contain("item2");

        result.Should()
              .Contain("item3");
    }

    [Test]
    public void RoundTrip_ComplexObjects_MaintainsData()
    {
        // Arrange
        var items = new[]
        {
            new TestItem
            {
                Id = 1,
                Name = "First"
            },
            new TestItem
            {
                Id = 2,
                Name = "Second"
            },
            new TestItem
            {
                Id = 3,
                Name = "Third"
            }
        };
        var original = new FixedSet<TestItem>(5, items);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<FixedSet<TestItem>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(original.Capacity);

        result.Count
              .Should()
              .Be(original.Count);

        var resultItems = result.ToList();
        var originalItems = original.ToList();

        for (var i = 0; i < originalItems.Count; i++)
        {
            resultItems[i]
                .Id
                .Should()
                .Be(originalItems[i].Id);

            resultItems[i]
                .Name
                .Should()
                .Be(originalItems[i].Name);
        }
    }

    [Test]
    public void RoundTrip_EmptyFixedSet_MaintainsData()
    {
        // Arrange
        var original = new FixedSet<int>(10);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<FixedSet<int>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(original.Capacity);

        result.Count
              .Should()
              .Be(0);
    }

    [Test]
    public void RoundTrip_SerializeAndDeserialize_MaintainsData()
    {
        // Arrange
        var original = new FixedSet<string>(
            5,
            [
                "test1",
                "test2",
                "test3"
            ]);

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<FixedSet<string>>(json, _options);

        // Assert
        result.Should()
              .NotBeNull();

        result.Capacity
              .Should()
              .Be(original.Capacity);

        result.Count
              .Should()
              .Be(original.Count);

        foreach (var item in original)
            result.Should()
                  .Contain(item);
    }

    [Test]
    public void Write_EmptyFixedSet_WritesCorrectJson()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(5);

        // Act
        var json = JsonSerializer.Serialize(fixedSet, _options);

        // Assert
        json.Should()
            .Be("{\"Capacity\":5,\"Items\":[]}");
    }

    [Test]
    public void Write_FixedSetWithComplexObjects_WritesCorrectJson()
    {
        // Arrange
        var items = new[]
        {
            new TestItem
            {
                Id = 1,
                Name = "First"
            },
            new TestItem
            {
                Id = 2,
                Name = "Second"
            }
        };
        var fixedSet = new FixedSet<TestItem>(5, items);

        // Act
        var json = JsonSerializer.Serialize(fixedSet, _options);

        // Assert
        json.Should()
            .Contain("\"Capacity\":5");

        json.Should()
            .Contain("\"Items\":");

        json.Should()
            .Contain("\"Id\":1");

        json.Should()
            .Contain("\"Name\":\"First\"");

        json.Should()
            .Contain("\"Id\":2");

        json.Should()
            .Contain("\"Name\":\"Second\"");
    }

    [Test]
    public void Write_FixedSetWithIntItems_WritesCorrectJson()
    {
        // Arrange
        var fixedSet = new FixedSet<int>(
            3,
            [
                1,
                2,
                3
            ]);

        // Act
        var json = JsonSerializer.Serialize(fixedSet, _options);

        // Assert
        json.Should()
            .Be("{\"Capacity\":3,\"Items\":[1,2,3]}");
    }

    [Test]
    public void Write_FixedSetWithItems_WritesCorrectJson()
    {
        // Arrange
        var fixedSet = new FixedSet<string>(
            3,
            [
                "item1",
                "item2",
                "item3"
            ]);

        // Act
        var json = JsonSerializer.Serialize(fixedSet, _options);

        // Assert
        json.Should()
            .Be("{\"Capacity\":3,\"Items\":[\"item1\",\"item2\",\"item3\"]}");
    }

    private sealed class TestItem
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}