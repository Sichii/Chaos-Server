#region
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class CompositeScriptTests
{
    [Test]
    public void Add_AddsScriptToComponents()
    {
        var compositeScriptBaase = MockCompositeScript.Create()
                                                      .Object;

        var script = MockScript.Create()
                               .Object;

        // Act
        compositeScriptBaase.Add(script);

        // Assert
        compositeScriptBaase.GetScripts<IScript>()
                            .Should()
                            .Contain(script);
    }

    [Test]
    public void Add_WrongType_ThrowsInvalidCastException()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        var act = () => composite.Add("not a script");

        act.Should()
           .Throw<InvalidCastException>();
    }

    [Test]
    public void GetEnumerator_EmptyScripts_YieldsNothing()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        composite.Should()
                 .BeEmpty();
    }

    [Test]
    public void GetEnumerator_ReturnsAllScripts()
    {
        // Arrange
        var compositeScript = MockCompositeScript.Create()
                                                 .Object;

        var scriptMock1 = MockScript.Create()
                                    .Object;

        var scriptMock2 = MockScript.Create()
                                    .Object;
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
        var script1 = MockScript.Create()
                                .Object;

        var script2 = MockScript.Create()
                                .Object;

        var subScript1 = MockScript.Create()
                                   .Object;

        var subScript2 = MockScript.Create()
                                   .Object;

        var compositeScript = MockCompositeScript.Create(subScript1, subScript2)
                                                 .Object;

        var mainComposite = MockCompositeScript.Create(script1, compositeScript, script2)
                                               .Object;

        // Assert
        mainComposite.Should()
                     .ContainInOrder(
                         script1,
                         subScript1,
                         subScript2,
                         script2);
    }

    [Test]
    public void GetScript_NoMatch_ReturnsNull()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        // ICompositeScript itself is not added — no match expected
        composite.GetScript<ICompositeScript>()
                 .Should()
                 .BeNull();
    }

    [Test]
    public void GetScript_ReturnsFirstInstanceOfType()
    {
        // Arrange
        var compositeScript = MockCompositeScript.Create()
                                                 .Object;

        var nestedCompositeScript = MockCompositeScript.Create()
                                                       .Object;

        var scriptMock1 = MockScript.Create()
                                    .Object;

        var scriptMock2 = MockScript.Create()
                                    .Object;
        nestedCompositeScript.Add(scriptMock1);
        nestedCompositeScript.Add(scriptMock2);
        compositeScript.Add(nestedCompositeScript);

        // Act
        var component = compositeScript.GetScript<ICompositeScript>();

        // Assert
        component.Should()
                 .BeSameAs(nestedCompositeScript);
    }

    [Test]
    public void GetScripts_EmptyComposite_YieldsNothing()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        composite.GetScripts<IScript>()
                 .Should()
                 .BeEmpty();
    }

    [Test]
    public void GetScripts_ReturnsAllInstancesOfType()
    {
        // Arrange
        var compositeScript = MockCompositeScript.Create()
                                                 .Object;

        var nestedCompositeScript = MockCompositeScript.Create()
                                                       .Object;

        var scriptMock1 = MockScript.Create()
                                    .Object;

        var scriptMock2 = MockScript.Create()
                                    .Object;
        nestedCompositeScript.Add(scriptMock1);
        nestedCompositeScript.Add(scriptMock2);
        compositeScript.Add(nestedCompositeScript);

        // Act
        var components = compositeScript.GetScripts<IScript>();

        // Assert
        components.Should()
                  .Contain(s => s == nestedCompositeScript);
    }

    [Test]
    public void Remove_NotPresent_IsNoOp()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        var script = MockScript.Create()
                               .Object;

        // Never added — removing should not throw
        var act = () => composite.Remove(script);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Remove_RemovesScriptFromComponents()
    {
        // Arrange
        var compositeScript = MockCompositeScript.Create()
                                                 .Object;

        var scriptMock = MockScript.Create()
                                   .Object;
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
        var compositeScript = MockCompositeScript.Create()
                                                 .Object;

        var nestedCompositeScript = MockCompositeScript.Create()
                                                       .Object;

        var scriptMock = MockScript.Create()
                                   .Object;
        nestedCompositeScript.Add(scriptMock);
        compositeScript.Add(nestedCompositeScript);

        // Act
        compositeScript.Remove(scriptMock);

        // Assert
        compositeScript.GetScripts<IScript>()
                       .Should()
                       .NotContain(scriptMock);
    }

    [Test]
    public void Remove_WrongType_ThrowsInvalidCastException()
    {
        var composite = MockCompositeScript.Create()
                                           .Object;

        var act = () => composite.Remove("not a script");

        act.Should()
           .Throw<InvalidCastException>();
    }
}