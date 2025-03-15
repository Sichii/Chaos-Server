#region
using Chaos.Collections.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class StaticVarsTests
{
    [Test]
    public void ContainsKey_ShouldReturnFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        var result = vars.ContainsKey("Key3");

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsKey_ShouldReturnTrue_WhenKeyExists()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        var result = vars.ContainsKey("Key1");

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Get_ShouldReturnDefaultValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        var result = vars.Get<int>("Key3");

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Get_ShouldReturnValue_WhenKeyExists()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        var result = vars.Get<string>("Key1");

        // Assert
        result.Should()
              .Be("Value1");
    }

    [Test]
    public void GetRequired_ShouldReturnValue_WhenKeyExists()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        var result = vars.GetRequired<string>("Key1");

        // Assert
        result.Should()
              .Be("Value1");
    }

    [Test]
    public void GetRequired_ShouldThrowException_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        Action act = () => vars.GetRequired<int>("Key3");

        // Assert
        act.Should()
           .Throw<KeyNotFoundException>()
           .WithMessage("Required key \"Key3\" was not found while populating script variables");
    }

    [Test]
    public void Set_ShouldSetValue_WhenKeyDoesNotExist()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                }
            });

        // Act
        vars.Set("Key2", 123);

        // Assert
        vars.Get<int>("Key2")
            .Should()
            .Be(123);
    }

    [Test]
    public void Set_ShouldUpdateValue_WhenKeyExists()
    {
        // Arrange
        var vars = new StaticVars(
            new Dictionary<string, object>
            {
                {
                    "Key1", "Value1"
                },
                {
                    "Key2", 123
                }
            });

        // Act
        vars.Set("Key2", 456);

        // Assert
        vars.Get<int>("Key2")
            .Should()
            .Be(456);
    }
}