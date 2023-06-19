using Chaos.Scripting.Abstractions.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class CompositeScriptTests
{
    [Fact]
    public void Add_AddsScriptToComponents()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock = new Mock<IScript>();

        // Act
        compositeScript.Add(scriptMock.Object);

        // Assert
        compositeScript.GetScripts<IScript>().Should().Contain(scriptMock.Object);
    }

    [Fact]
    public void GetComponent_ReturnsFirstInstanceOfType()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock1 = new Mock<IScript>();
        var scriptMock2 = new Mock<IScript>();
        compositeScript.Add(scriptMock1.Object);
        compositeScript.Add(scriptMock2.Object);

        // Act
        var component = compositeScript.GetScript<IScript>();

        // Assert
        component.Should().Be(scriptMock1.Object);
    }

    [Fact]
    public void GetComponents_ReturnsAllInstancesOfType()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock1 = new Mock<IScript>();
        var scriptMock2 = new Mock<IScript>();
        compositeScript.Add(scriptMock1.Object);
        compositeScript.Add(scriptMock2.Object);

        // Act
        var components = compositeScript.GetScripts<IScript>();

        // Assert
        components.Should().Contain(new[] { scriptMock1.Object, scriptMock2.Object });
    }

    [Fact]
    public void GetEnumerator_ReturnsAllScripts()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock1 = new Mock<IScript>();
        var scriptMock2 = new Mock<IScript>();
        compositeScript.Add(scriptMock1.Object);
        compositeScript.Add(scriptMock2.Object);

        // Act
        var scripts = compositeScript.ToList();

        // Assert

        scripts.Should().Contain(new[] { scriptMock1.Object, scriptMock2.Object });
    }

    [Fact]
    public void Remove_RemovesScriptFromComponents()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock = new Mock<IScript>();
        compositeScript.Add(scriptMock.Object);

        // Act
        compositeScript.Remove(scriptMock.Object);

        // Assert
        compositeScript.GetScripts<IScript>().Should().NotContain(scriptMock.Object);
    }
}