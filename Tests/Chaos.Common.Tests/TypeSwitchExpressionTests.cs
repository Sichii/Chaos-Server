using Chaos.Common.Utilities;
using FluentAssertions;

namespace Chaos.Common.Tests;

public sealed class TypeSwitchExpressionTests
{
    [Test]
    public void Case_DuplicateType_ShouldThrow()
    {
        var tse = new TypeSwitchExpression<string>().Case<int>("one");

        Action act = () => tse.Case<int>("two");

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Case_ObjectOverload_ShouldCastAndReturn()
    {
        var tse = new TypeSwitchExpression<int>().Case<int>((object)7)
                                                 .Default(0);

        tse.Switch<int>()
           .Should()
           .Be(7);
    }

    [Test]
    public void Default_ObjectOverload_ShouldCastAndReturn()
    {
        var tse = new TypeSwitchExpression<string>().Default((object)"x");

        tse.Switch<int>()
           .Should()
           .Be("x");
    }

    [Test]
    public void Freeze_ShouldReturnFrozenVariantThatStillWorks()
    {
        var tse = new TypeSwitchExpression<string>().Case<int>("i")
                                                    .Case<double>(() => "d")
                                                    .Default("x");

        var frozen = tse.Freeze();

        frozen.Switch<int>()
              .Should()
              .Be("i");

        frozen.Switch<double>()
              .Should()
              .Be("d");

        frozen.Switch<decimal>()
              .Should()
              .Be("x");
    }

    [Test]
    public void Switch_WithMatchingCase_ShouldReturnValue()
    {
        var tse = new TypeSwitchExpression<string>().Case<int>("int")
                                                    .Case<string>("string")
                                                    .Default("default");

        tse.Switch<int>()
           .Should()
           .Be("int");

        tse.Switch<string>()
           .Should()
           .Be("string");
    }

    [Test]
    public void Switch_WithNoMatch_ShouldUseDefault()
    {
        var tse = new TypeSwitchExpression<int>().Case<int>(1)
                                                 .Default(42);

        tse.Switch<double>()
           .Should()
           .Be(42);
    }
}