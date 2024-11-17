#region
using Chaos.Scripting.Abstractions.Tests.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class CompositeScriptTests
{
    [Test]
    public void Add_AddsScriptToComponents()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock = new MockScript();

        // Act
        compositeScript.Add(scriptMock);

        // Assert
        compositeScript.GetScripts<IScript>()
                       .Should()
                       .Contain(scriptMock);
    }

    [Test]
    public void GetEnumerator_ReturnsAllScripts()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock1 = new MockScript();
        var scriptMock2 = new MockScript();
        compositeScript.Add(scriptMock1);
        compositeScript.Add(scriptMock2);

        // Act
        var scripts = compositeScript.ToList();

        // Assert

        scripts.Should()
               .Contain(
                   [
                       scriptMock1,
                       scriptMock2
                   ]);
    }

    [Test]
    public void GetEnumerator_YieldsCompositeScriptsAndSubscripts()
    {
        // Arrange
        var script1 = new MockScript();
        var script2 = new MockScript();
        var subScript1 = new MockScript();
        var subScript2 = new MockScript();

        var compositeScript = new MockCompositeScript
        {
            subScript1,
            subScript2
        };

        var mainComposite = new MockCompositeScript
        {
            script1,
            compositeScript,
            script2
        };

        // Assert
        mainComposite.Should()
                     .ContainInOrder(
                         script1,
                         compositeScript,
                         subScript1,
                         subScript2,
                         script2);
    }

    [Test]
    public void GetScript_ReturnsFirstInstanceOfType()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var nestedCompositeScript = new MockCompositeScript();
        var scriptMock1 = new MockScript();
        var scriptMock2 = new MockScript();
        nestedCompositeScript.Add(scriptMock1);
        nestedCompositeScript.Add(scriptMock2);
        compositeScript.Add(nestedCompositeScript);

        // Act
        var component = compositeScript.GetScript<MockScript>();

        // Assert
        component.Should()
                 .Be(scriptMock1);
    }

    [Test]
    public void GetScripts_ReturnsAllInstancesOfType()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var nestedCompositeScript = new MockCompositeScript();
        var scriptMock1 = new MockScript();
        var scriptMock2 = new MockScript();
        nestedCompositeScript.Add(scriptMock1);
        nestedCompositeScript.Add(scriptMock2);
        compositeScript.Add(nestedCompositeScript);

        // Act
        var components = compositeScript.GetScripts<MockScript>();

        // Assert
        components.Should()
                  .Contain(
                      [
                          scriptMock1,
                          scriptMock2
                      ]);
    }

    [Test]
    public void Remove_RemovesScriptFromComponents()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var scriptMock = new MockScript();
        compositeScript.Add(scriptMock);

        // Act
        compositeScript.Remove(scriptMock);

        // Assert
        compositeScript.GetScripts<IScript>()
                       .Should()
                       .NotContain(scriptMock);
    }

    [Test]
    public void Remove_RemovesScriptFromNestedComponents()
    {
        // Arrange
        var compositeScript = new MockCompositeScript();
        var nestedCompositeScript = new MockCompositeScript();
        var scriptMock = new MockScript();
        nestedCompositeScript.Add(scriptMock);
        compositeScript.Add(nestedCompositeScript);

        // Act
        compositeScript.Remove(scriptMock);

        // Assert
        compositeScript.GetScripts<IScript>()
                       .Should()
                       .NotContain(scriptMock);
    }
}