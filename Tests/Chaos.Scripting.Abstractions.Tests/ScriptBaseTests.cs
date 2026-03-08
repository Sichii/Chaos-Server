#region
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class ScriptBaseTests
{
    [Test]
    public void Equals_Object_WithNonScriptBaseType_ReturnsFalse()
    {
        // Arrange
        var script = new MockScriptBase();

        // Assert
        script.Equals("not a script base")
              .Should()
              .BeFalse();
    }

    [Test]
    public void Equals_Object_WithNull_ReturnsFalse()
    {
        // Arrange
        var script = new MockScriptBase();

        // Assert
        script.Equals((object?)null)
              .Should()
              .BeFalse();
    }

    [Test]
    public void Equals_ReturnsFalse_ForDifferentObjects()
    {
        // Arrange
        var scriptA = MockScript.Create()
                                .Object;

        var scriptB = MockCompositeScript.Create()
                                         .Object;

        // Assert
        scriptA.Equals(scriptB)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrue_ForObjectsWithSameScriptKey()
    {
        // Arrange
        var script1 = new MockScriptBase();
        var script2 = new MockScriptBase();

        // Assert
        script1.Equals(script2)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Equals_ReturnsTrue_ForSameObjects()
    {
        // Arrange
        var script = new MockScriptBase();

        // Assert
        script.Equals(script)
              .Should()
              .BeTrue();
    }

    [Test]
    public void GetHashCode_ReturnsSameValue_ForObjectsWithSameScriptKey()
    {
        // Arrange
        var script1 = new MockScriptBase();
        var script2 = new MockScriptBase();

        // Assert
        script1.GetHashCode()
               .Should()
               .Be(script2.GetHashCode());
    }

    [Test]
    public void GetScriptKey_WithoutScriptSuffix_ReturnsFullName()
    {
        // Arrange
        var type = typeof(SomeThing);

        // Act
        var key = ScriptBase.GetScriptKey(type);

        // Assert
        key.Should()
           .Be("SomeThing");
    }

    [Test]
    public void GetScriptKey_WithScriptSuffix_RemovesSuffix()
    {
        // Arrange
        var type = typeof(FooBarScript);

        // Act
        var key = ScriptBase.GetScriptKey(type);

        // Assert
        key.Should()
           .Be("FooBar");
    }

    // Helper types for GetScriptKey tests — names are intentional
    private sealed class FooBarScript : ScriptBase { }

    private sealed class SomeThing : ScriptBase { }
}