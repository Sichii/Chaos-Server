#region
using System.Collections;
using System.Text.Json;
using Chaos.Collections.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class DynamicVarsTests
{
    private readonly JsonSerializerOptions _jsonOptions = new();

    [Test]
    public void Constructor_WithCollectionAndOptions_ShouldInitializeCorrectly()
    {
        // Arrange
        var initialData = new Dictionary<string, JsonElement>
        {
            ["testKey"] = JsonSerializer.SerializeToElement("testValue"),
            ["numberKey"] = JsonSerializer.SerializeToElement(42)
        };

        // Act
        var dynamicVars = new DynamicVars(initialData, _jsonOptions);

        // Assert
        dynamicVars.ContainsKey("testKey")
                   .Should()
                   .BeTrue();

        dynamicVars.ContainsKey("numberKey")
                   .Should()
                   .BeTrue();

        dynamicVars.ContainsKey("nonExistentKey")
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void Constructor_WithCollectionValueCacheAndOptions_ShouldInitializeCorrectly()
    {
        // Arrange
        var initialData = new Dictionary<string, JsonElement>
        {
            ["testKey"] = JsonSerializer.SerializeToElement("testValue")
        };

        var valueCache = new Dictionary<string, object?>
        {
            ["testKey"] = "cachedValue"
        };

        // Act
        var dynamicVars = new DynamicVars(initialData, valueCache, _jsonOptions);

        // Assert
        dynamicVars.ContainsKey("testKey")
                   .Should()
                   .BeTrue();

        dynamicVars.Get<string>("testKey")
                   .Should()
                   .Be("cachedValue");
    }

    [Test]
    public void Constructor_WithOptionsOnly_ShouldInitializeEmpty()
    {
        // Act
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Assert
        dynamicVars.ContainsKey("anyKey")
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void ContainsKey_IsCaseInsensitive()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("TestKey", "value");

        // Act & Assert
        dynamicVars.ContainsKey("testkey")
                   .Should()
                   .BeTrue();

        dynamicVars.ContainsKey("TESTKEY")
                   .Should()
                   .BeTrue();

        dynamicVars.ContainsKey("TestKey")
                   .Should()
                   .BeTrue();
    }

    [Test]
    public void ContainsKey_WithExistingKey_ShouldReturnTrue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("existingKey", "value");

        // Act & Assert
        dynamicVars.ContainsKey("existingKey")
                   .Should()
                   .BeTrue();
    }

    [Test]
    public void ContainsKey_WithNonExistingKey_ShouldReturnFalse()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Act & Assert
        dynamicVars.ContainsKey("nonExistingKey")
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void Get_IsCaseInsensitive()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("TestKey", "value");

        // Act & Assert
        dynamicVars.Get<string>("testkey")
                   .Should()
                   .Be("value");

        dynamicVars.Get<string>("TESTKEY")
                   .Should()
                   .Be("value");
    }

    [Test]
    public void Get_ShouldUseCache_WhenCalledMultipleTimes()
    {
        // Arrange
        var initialData = new Dictionary<string, JsonElement>
        {
            ["testKey"] = JsonSerializer.SerializeToElement("testValue")
        };
        var dynamicVars = new DynamicVars(initialData, _jsonOptions);

        // Act - First call should populate cache
        var firstResult = dynamicVars.Get<string>("testKey");
        var secondResult = dynamicVars.Get<string>("testKey");

        // Assert
        firstResult.Should()
                   .Be("testValue");

        secondResult.Should()
                    .Be("testValue");

        ReferenceEquals(firstResult, secondResult)
            .Should()
            .BeTrue(); // Should be same cached instance for strings
    }

    [Test]
    public void Get_WithExistingKey_ShouldReturnCorrectValue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("stringKey", "testValue");
        dynamicVars.Set("intKey", 42);
        dynamicVars.Set("boolKey", true);

        // Act & Assert
        dynamicVars.Get<string>("stringKey")
                   .Should()
                   .Be("testValue");

        dynamicVars.Get<int>("intKey")
                   .Should()
                   .Be(42);

        dynamicVars.Get<bool>("boolKey")
                   .Should()
                   .BeTrue();
    }

    [Test]
    public void Get_WithNonExistingKey_ShouldReturnDefault()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Act & Assert
        dynamicVars.Get<string>("nonExistingKey")
                   .Should()
                   .BeNull();

        dynamicVars.Get<int>("nonExistingKey")
                   .Should()
                   .Be(0);

        dynamicVars.Get<bool>("nonExistingKey")
                   .Should()
                   .BeFalse();
    }

    [Test]
    public void Get_WithTypeParameter_NonExistingKey_ShouldReturnNull()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Act
        var result = dynamicVars.Get(typeof(string), "nonExistingKey");

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void Get_WithTypeParameter_ShouldReturnCorrectValue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("testKey", "testValue");

        // Act
        var result = dynamicVars.Get(typeof(string), "testKey");

        // Assert
        result.Should()
              .Be("testValue");
    }

    [Test]
    public void GetEnumerator_NonGeneric_ShouldReturnAllKeyValuePairs()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("key1", "value1");

        // Act
        IEnumerable enumerable = dynamicVars;
        var enumerator = enumerable.GetEnumerator();
        using var enumerator1 = enumerator as IDisposable;
        var items = new List<object>();

        while (enumerator.MoveNext())
            items.Add(enumerator.Current!);

        // Assert
        items.Should()
             .HaveCount(1);

        items[0]
            .Should()
            .BeOfType<KeyValuePair<string, JsonElement>>();
    }

    [Test]
    public void GetEnumerator_ShouldReturnAllKeyValuePairs()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("key1", "value1");
        dynamicVars.Set("key2", "value2");

        // Act
        var pairs = dynamicVars.ToList();

        // Assert
        pairs.Should()
             .HaveCount(2);

        pairs.Should()
             .Contain(kvp => kvp.Key == "key1");

        pairs.Should()
             .Contain(kvp => kvp.Key == "key2");
    }

    [Test]
    public void GetRequired_IsCaseInsensitive()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("TestKey", "value");

        // Act & Assert
        dynamicVars.GetRequired<string>("testkey")
                   .Should()
                   .Be("value");

        dynamicVars.GetRequired<string>("TESTKEY")
                   .Should()
                   .Be("value");
    }

    [Test]
    public void GetRequired_WithExistingKey_ShouldReturnValue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("testKey", "testValue");

        // Act
        var result = dynamicVars.GetRequired<string>("testKey");

        // Assert
        result.Should()
              .Be("testValue");
    }

    [Test]
    public void GetRequired_WithNonExistingKey_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Act & Assert
        var action = () => dynamicVars.GetRequired<string>("nonExistingKey");

        action.Should()
              .Throw<KeyNotFoundException>()
              .WithMessage("Required key \"nonExistingKey\" was not found while populating script variables");
    }

    [Test]
    public void Set_ShouldAddNewKeyValue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        // Act
        dynamicVars.Set("newKey", "newValue");

        // Assert
        dynamicVars.ContainsKey("newKey")
                   .Should()
                   .BeTrue();

        dynamicVars.Get<string>("newKey")
                   .Should()
                   .Be("newValue");
    }

    [Test]
    public void Set_ShouldUpdateExistingKeyValue()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("testKey", "originalValue");

        // Act
        dynamicVars.Set("testKey", "updatedValue");

        // Assert
        dynamicVars.Get<string>("testKey")
                   .Should()
                   .Be("updatedValue");
    }

    [Test]
    public void Set_WithComplexObject_ShouldSerializeCorrectly()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);

        var complexObject = new
        {
            Name = "Test",
            Age = 30,
            IsActive = true
        };

        // Act
        dynamicVars.Set("complexKey", complexObject);

        // Assert
        dynamicVars.ContainsKey("complexKey")
                   .Should()
                   .BeTrue();
        var retrieved = dynamicVars.Get<object>("complexKey");

        retrieved.Should()
                 .NotBeNull();
    }

    [Test]
    public void ToStaticVars_WithBooleanValues_ShouldConvertCorrectly()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("trueKey", true);
        dynamicVars.Set("falseKey", false);

        // Act
        var staticVars = dynamicVars.ToStaticVars();

        // Assert
        staticVars.Should()
                  .NotBeNull();

        staticVars.Get<bool>("trueKey")
                  .Should()
                  .BeTrue();

        staticVars.Get<bool>("falseKey")
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void ToStaticVars_WithMixedValueTypes_ShouldConvertAllCorrectly()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("stringKey", "text");
        dynamicVars.Set("numberKey", 123.45);
        dynamicVars.Set("boolKey", true);

        // Act
        var staticVars = dynamicVars.ToStaticVars();

        // Assert
        staticVars.Should()
                  .NotBeNull();

        staticVars.Get<string>("stringKey")
                  .Should()
                  .Be("text");

        staticVars.Get<double>("numberKey")
                  .Should()
                  .Be(123.45);

        staticVars.Get<bool>("boolKey")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void ToStaticVars_WithNullValues_ShouldConvertCorrectly()
    {
        // Arrange
        var initialData = new Dictionary<string, JsonElement>
        {
            ["nullKey"] = JsonSerializer.SerializeToElement<object?>(null)
        };
        var dynamicVars = new DynamicVars(initialData, _jsonOptions);

        // Act
        var staticVars = dynamicVars.ToStaticVars();

        // Assert
        staticVars.Should()
                  .NotBeNull();

        staticVars.Get<object>("nullKey")
                  .Should()
                  .BeNull();
    }

    [Test]
    public void ToStaticVars_WithNumericValues_ShouldConvertCorrectly()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("numberKey", 42.5);

        // Act
        var staticVars = dynamicVars.ToStaticVars();

        // Assert
        staticVars.Should()
                  .NotBeNull();

        staticVars.Get<double>("numberKey")
                  .Should()
                  .Be(42.5);
    }

    [Test]
    public void ToStaticVars_WithStringValues_ShouldConvertCorrectly()
    {
        // Arrange
        var dynamicVars = new DynamicVars(_jsonOptions);
        dynamicVars.Set("stringKey", "testValue");

        // Act
        var staticVars = dynamicVars.ToStaticVars();

        // Assert
        staticVars.Should()
                  .NotBeNull();

        staticVars.Get<string>("stringKey")
                  .Should()
                  .Be("testValue");
    }

    [Test]
    public void ToStaticVars_WithUnsupportedJsonValueKind_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var complexObject = new
        {
            nested = new
            {
                value = "test"
            }
        };
        var jsonElement = JsonSerializer.SerializeToElement(complexObject);

        var initialData = new Dictionary<string, JsonElement>
        {
            ["complexKey"] = jsonElement
        };
        var dynamicVars = new DynamicVars(initialData, _jsonOptions);

        // Act & Assert
        var action = () => dynamicVars.ToStaticVars();

        action.Should()
              .Throw<ArgumentOutOfRangeException>();
    }
}