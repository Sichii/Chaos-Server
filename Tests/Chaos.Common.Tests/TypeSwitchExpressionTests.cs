#region
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class TypeSwitchExpressionTests
{
    [Test]
    public void GenericSwitch_ShouldExecuteFunctionForMatchingType()
    {
        // Arrange
        var typeSwitch = new TypeSwitchExpression<int>().Case<int>(() => 12345);

        // Act
        var result = typeSwitch.Switch<int>();

        // Assert
        result.Should()
              .Be(12345);
    }

    [Test]
    public void Switch_ShouldExecuteDefaultFunctionWhenNoMatchFound()
    {
        // Arrange
        var typeSwitch = new TypeSwitchExpression<int>().Case<string>(() => 123)
                                                        .Default(() => 789);

        // Act
        var result = typeSwitch.Switch(typeof(double));

        // Assert
        result.Should()
              .Be(789);
    }

    [Test]
    public void Switch_ShouldExecuteFunctionForMatchingType()
    {
        // Arrange
        var typeSwitch = new TypeSwitchExpression<string>().Case<string>(() => "Hello, World!");

        // Act
        var result = typeSwitch.Switch(typeof(string));

        // Assert
        result.Should()
              .Be("Hello, World!");
    }

    [Test]
    public void Switch_ShouldReturnDefaultValueWhenNoMatchFound()
    {
        // Arrange
        var typeSwitch = new TypeSwitchExpression<int>().Case<string>(() => 123)
                                                        .Default(456);

        // Act
        var result = typeSwitch.Switch(typeof(double));

        // Assert
        result.Should()
              .Be(456);
    }

    [Test]
    public void Switch_ShouldThrowInvalidOperationExceptionWhenNoMatchAndNoDefault()
    {
        // Arrange
        var typeSwitch = new TypeSwitchExpression<int>().Case<string>(() => 123);

        // Act
        Action act = () => typeSwitch.Switch(typeof(double));

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("No case was matched");
    }
}