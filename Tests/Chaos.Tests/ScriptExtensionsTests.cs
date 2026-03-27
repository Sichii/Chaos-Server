#region
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Chaos.Extensions;
using Chaos.Scripting.Abstractions;
using FluentAssertions;
using Moq;
#endregion

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Chaos.Tests;

internal interface ITestScript : IScript { }

public sealed class ScriptExtensionsTests
{
    #region As
    [Test]
    public void As_ShouldReturnScript_WhenDirectTypeMatch()
    {
        var testScript = new Mock<ITestScript>();
        IScript script = testScript.Object;

        var result = script.As<ITestScript>();

        result.Should()
              .BeSameAs(testScript.Object);
    }

    [Test]
    public void As_ShouldReturnScript_WhenFoundInComposite()
    {
        var innerScript = new Mock<ITestScript>();
        var compositeMock = new Mock<IScript>();

        compositeMock.As<ICompositeScript>()
                     .Setup(c => c.GetScript<ITestScript>())
                     .Returns(innerScript.Object);

        var script = compositeMock.Object;

        var result = script.As<ITestScript>();

        result.Should()
              .BeSameAs(innerScript.Object);
    }

    [Test]
    public void As_ShouldReturnDefault_WhenNotMatchedAndNotComposite()
    {
        var plainScript = new Mock<IScript>();
        var script = plainScript.Object;

        var result = script.As<ITestScript>();

        result.Should()
              .BeNull();
    }
    #endregion

    #region Is
    [Test]
    public void Is_ShouldReturnTrue_WhenTypeMatches()
    {
        var testScript = new Mock<ITestScript>();
        IScript script = testScript.Object;

        script.Is<ITestScript>()
              .Should()
              .BeTrue();
    }

    [Test]
    public void Is_ShouldReturnFalse_WhenTypeDoesNotMatch()
    {
        var plainScript = new Mock<IScript>();
        var script = plainScript.Object;

        script.Is<ITestScript>()
              .Should()
              .BeFalse();
    }

    [Test]
    public void Is_ShouldReturnTrue_WhenFoundInComposite()
    {
        var innerScript = new Mock<ITestScript>();
        var compositeMock = new Mock<IScript>();

        compositeMock.As<ICompositeScript>()
                     .Setup(c => c.GetScript<ITestScript>())
                     .Returns(innerScript.Object);

        var script = compositeMock.Object;

        script.Is<ITestScript>()
              .Should()
              .BeTrue();
    }
    #endregion

    #region Is (out parameter)
    [Test]
    public void IsOut_ShouldReturnTrue_WithScript_WhenFound()
    {
        var testScript = new Mock<ITestScript>();
        IScript script = testScript.Object;

        script.Is<ITestScript>(out var found)
              .Should()
              .BeTrue();

        found.Should()
             .BeSameAs(testScript.Object);
    }

    [Test]
    public void IsOut_ShouldReturnFalse_WithNull_WhenNotFound()
    {
        var plainScript = new Mock<IScript>();
        var script = plainScript.Object;

        script.Is<ITestScript>(out var found)
              .Should()
              .BeFalse();

        found.Should()
             .BeNull();
    }
    #endregion

    #region GetScripts
    [Test]
    public void GetScripts_ShouldDelegateToComposite()
    {
        var innerScript1 = new Mock<ITestScript>();
        var innerScript2 = new Mock<ITestScript>();

        var expected = new[]
        {
            innerScript1.Object,
            innerScript2.Object
        };

        var compositeMock = new Mock<IScript>();

        compositeMock.As<ICompositeScript>()
                     .Setup(c => c.GetScripts<ITestScript>())
                     .Returns(expected);

        var script = compositeMock.Object;

        var result = script.GetScripts<ITestScript>()
                           .ToList();

        result.Should()
              .HaveCount(2);

        result.Should()
              .Contain(innerScript1.Object);

        result.Should()
              .Contain(innerScript2.Object);
    }

    [Test]
    public void GetScripts_ShouldReturnEmpty_WhenNotComposite()
    {
        var plainScript = new Mock<IScript>();
        var script = plainScript.Object;

        var result = script.GetScripts<ITestScript>();

        result.Should()
              .BeEmpty();
    }
    #endregion

    #region RemoveScript
    [Test]
    public void RemoveScript_ShouldRemoveFromComposite_WhenFound()
    {
        var innerScript = new Mock<ITestScript>();

        var compositeMock = new Mock<IScript>();

        compositeMock.As<ICompositeScript>()
                     .Setup(c => c.GetScript<ITestScript>())
                     .Returns(innerScript.Object);

        var scriptedMock = new Mock<IScripted<IScript>>();

        scriptedMock.SetupGet(s => s.Script)
                    .Returns(compositeMock.Object);

        scriptedMock.SetupGet(s => s.ScriptKeys)
                    .Returns(new HashSet<string>());

        scriptedMock.Object.RemoveScript<ITestScript>();

        compositeMock.As<ICompositeScript>()
                     .Verify(c => c.Remove(innerScript.Object), Times.Once);
    }

    [Test]
    public void RemoveScript_ShouldDoNothing_WhenScriptNotFound()
    {
        var plainScript = new Mock<IScript>();

        var scriptedMock = new Mock<IScripted<IScript>>();

        scriptedMock.SetupGet(s => s.Script)
                    .Returns(plainScript.Object);

        scriptedMock.Object.RemoveScript<ITestScript>();

        // Should return early without any side effects
        plainScript.VerifyNoOtherCalls();
    }

    [Test]
    public void RemoveScript_ShouldThrow_WhenScriptFoundButNotComposite()
    {
        // Edge case: script IS the type being removed, but is not a composite
        var testScript = new Mock<ITestScript>();

        var scriptedMock = new Mock<IScripted<IScript>>();

        scriptedMock.SetupGet(s => s.Script)
                    .Returns(testScript.Object);

        var act = () => scriptedMock.Object.RemoveScript<ITestScript>();

        act.Should()
           .Throw<UnreachableException>();
    }
    #endregion
}