#region
using Chaos.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class DialogStringTests
{
    #region Format
    [Test]
    public void Format_ShouldInjectParameters()
    {
        var result = DialogString.Format("{0} hello {1}", "a", "b");

        result.Should()
              .Be("a hello b");
    }
    #endregion

    #region From
    [Test]
    public void From_ShouldCreateLazy()
    {
        var lazy = DialogString.From(() => "test value");

        lazy.Should()
            .NotBeNull();

        lazy.IsValueCreated
            .Should()
            .BeFalse();

        lazy.Value
            .Should()
            .Be("test value");

        lazy.IsValueCreated
            .Should()
            .BeTrue();
    }
    #endregion

    #region Properties
    [Test]
    public void No_ShouldReturnNo()
        => DialogString.No
                       .Should()
                       .Be("No");

    [Test]
    public void Ok_ShouldReturnOk()
        => DialogString.Ok
                       .Should()
                       .Be("Ok");

    [Test]
    public void UnknownInput_ShouldReturnExpectedString()
        => DialogString.UnknownInput
                       .Should()
                       .Be("Huh...? I'm not sure what you mean...");

    [Test]
    public void Yes_ShouldReturnYes()
        => DialogString.Yes
                       .Should()
                       .Be("Yes");
    #endregion
}