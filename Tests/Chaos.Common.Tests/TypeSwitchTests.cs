#region
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class TypeSwitchTests
{
    [Test]
    public void GenericSwitch_ShouldExecuteActionForMatchingType()
    {
        // Arrange
        var intCaseExecuted = false;

        var typeSwitch = new TypeSwitch().Case<int>(() => intCaseExecuted = true);

        // Act
        typeSwitch.Switch<int>();

        // Assert
        intCaseExecuted.Should()
                       .BeTrue();
    }

    [Test]
    public void Switch_ShouldExecuteActionForMatchingType()
    {
        // Arrange
        var stringCaseExecuted = false;

        var typeSwitch = new TypeSwitch().Case<string>(() => stringCaseExecuted = true);

        // Act
        typeSwitch.Switch(typeof(string));

        // Assert
        stringCaseExecuted.Should()
                          .BeTrue();
    }

    [Test]
    public void Switch_ShouldExecuteDefaultActionWhenNoMatchFound()
    {
        // Arrange
        var defaultCaseExecuted = false;

        var typeSwitch = new TypeSwitch().Case<string>(() => { })
                                         .Default(() => defaultCaseExecuted = true);

        // Act
        typeSwitch.Switch(typeof(int));

        // Assert
        defaultCaseExecuted.Should()
                           .BeTrue();
    }

    [Test]
    public void Switch_ShouldThrowInvalidOperationExceptionWhenNoMatchAndNoDefault()
    {
        // Arrange
        var typeSwitch = new TypeSwitch().Case<string>(() => { });

        // Act
        var act = () => typeSwitch.Switch(typeof(int));

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("No case was matched");
    }
}