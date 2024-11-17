#region
using Chaos.Scripting.Abstractions.Tests.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class ConfigurableScriptTests
{
      [Test]
    public void Ctor_DoesNotSetPropertiesIfNoScriptVarsForKey()
    {
        // Arrange
        var scriptVars = new MockScriptVars();
        scriptVars.Set("TestValue", "NonExistentKey");
        scriptVars.Set(42, "NonExistentKey");
        var subject = new MockScripted();

        // Act
        var script = new MockConfigurableScript(subject, scriptVars);

        // Assert
        script.StringProp
              .Should()
              .BeNull();

        script.IntProp
              .Should()
              .Be(0);
    }

      [Test]
    public void Ctor_SetsPropertiesFromScriptVars()
    {
        // Arrange
        var scriptVars = new MockScriptVars();
        scriptVars.Set("TestValue", "StringProp");
        scriptVars.Set(42, "IntProp");
        var subject = new MockScripted();

        // Act
        var script = new MockConfigurableScript(subject, scriptVars);

        // Assert
        script.StringProp
              .Should()
              .Be("TestValue");

        script.IntProp
              .Should()
              .Be(42);
    }

      [Test]
    public void Ctor_WithFactory_SetsPropertiesFromScriptVars()
    {
        // Arrange
        var scriptVars = new MockScriptVars();
        scriptVars.Set("TestValue", "StringProp");
        scriptVars.Set(42, "IntProp");
        var subject = new MockScripted();

        // Act
        var script = new MockConfigurableScript(subject, _ => scriptVars);

        // Assert
        script.StringProp
              .Should()
              .Be("TestValue");

        script.IntProp
              .Should()
              .Be(42);
    }

      [Test]
    public void Ctor_WithFactory_ThrowsIfFactoryReturnsNull()
    {
        // Arrange
        var subject = new MockScripted();

        // Act
        // ReSharper disable once ObjectCreationAsStatement
        #pragma warning disable CA1806
        Action act = () => new MockConfigurableScript(subject, _ => null!);
        #pragma warning restore CA1806

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }
}