#region
using System.Text.Json;
using Chaos.Collections.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class DynamicVarsTests
{
    [Test]
    public void ContainsKey_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new DynamicVars(new Dictionary<string, JsonElement>(), new JsonSerializerOptions());

        // Act
        var result = vars.ContainsKey("key");

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var collection = new Dictionary<string, JsonElement>
        {
            ["key"] = JsonDocument.Parse("\"value\"")
                                  .RootElement
        };

        var vars = new DynamicVars(collection, new JsonSerializerOptions());

        // Act
        var result = vars.ContainsKey("key");

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Get_ShouldReturnDefaultValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new DynamicVars(new Dictionary<string, JsonElement>(), new JsonSerializerOptions());

        // Act
        var intValue = vars.Get<int>("integer");
        var boolValue = vars.Get<bool>("boolean");
        var dateTimeValue = vars.Get<DateTime>("dateTime");
        var nullableValue = vars.Get<int?>("nullable");

        // Assert
        intValue.Should()
                .Be(0);

        boolValue.Should()
                 .BeFalse();

        dateTimeValue.Should()
                     .Be(DateTime.MinValue);

        nullableValue.Should()
                     .BeNull();
    }

    [Test]
    public void Get_ShouldReturnTypedValue_WhenKeyExists()
    {
        // Arrange
        var collection = new Dictionary<string, JsonElement>
        {
            ["integer"] = JsonDocument.Parse("42")
                                      .RootElement,
            ["boolean"] = JsonDocument.Parse("true")
                                      .RootElement,
            ["dateTime"] = JsonDocument.Parse("\"2022-01-01T12:34:56\"")
                                       .RootElement,
            ["nullable"] = JsonDocument.Parse("null")
                                       .RootElement
        };

        var options = new JsonSerializerOptions();
        var vars = new DynamicVars(collection, options);

        // Act
        var intValue = vars.Get<int>("integer");
        var boolValue = vars.Get<bool>("boolean");
        var dateTimeValue = vars.Get<DateTime>("dateTime");
        var nullableValue = vars.Get<int?>("nullable");

        // Assert
        intValue.Should()
                .Be(42);

        boolValue.Should()
                 .BeTrue();

        dateTimeValue.Should()
                     .Be(
                         new DateTime(
                             2022,
                             1,
                             1,
                             12,
                             34,
                             56));

        nullableValue.Should()
                     .BeNull();
    }

    [Test]
    public void GetRequired_ShouldReturnValue_WhenKeyExists()
    {
        // Arrange
        var collection = new Dictionary<string, JsonElement>
        {
            ["integer"] = JsonDocument.Parse("42")
                                      .RootElement,
            ["boolean"] = JsonDocument.Parse("true")
                                      .RootElement,
            ["dateTime"] = JsonDocument.Parse("\"2022-01-01T12:34:56\"")
                                       .RootElement,
            ["nullable"] = JsonDocument.Parse("null")
                                       .RootElement
        };

        var options = new JsonSerializerOptions();
        var vars = new DynamicVars(collection, options);

        // Act
        var intValue = vars.GetRequired<int>("integer");
        var boolValue = vars.GetRequired<bool>("boolean");
        var dateTimeValue = vars.GetRequired<DateTime>("dateTime");
        var nullableValue = vars.GetRequired<int?>("nullable");

        // Assert
        intValue.Should()
                .Be(42);

        boolValue.Should()
                 .BeTrue();

        dateTimeValue.Should()
                     .Be(
                         new DateTime(
                             2022,
                             1,
                             1,
                             12,
                             34,
                             56));

        nullableValue.Should()
                     .BeNull();
    }

    [Test]
    public void GetRequired_ShouldThrowException_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new DynamicVars(new Dictionary<string, JsonElement>(), new JsonSerializerOptions());

        // Act
        Action act = () => vars.GetRequired<int>("integer");

        // Assert
        act.Should()
           .Throw<KeyNotFoundException>()
           .WithMessage("Required key \"integer\" was not found while populating script variables");
    }
}