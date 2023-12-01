using Chaos.Scripting.Abstractions.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class ScriptBaseTests
{
    [Fact]
    public void Equals_ReturnsFalse_ForDifferentObjects()
    {
        // Arrange
        var scriptA = new MockScriptBase();
        var scriptB = new MockCompositeScript();

        // Assert
        scriptA.Equals(scriptB)
               .Should()
               .BeFalse();
    }

    [Fact]
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

    [Fact]
    public void Equals_ReturnsTrue_ForSameObjects()
    {
        // Arrange
        var script = new MockScriptBase();

        // Assert
        script.Equals(script)
              .Should()
              .BeTrue();
    }

    [Fact]
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
}