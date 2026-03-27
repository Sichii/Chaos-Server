#region
using System.ComponentModel;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class ObjectExtensionsTests
{
    [Test]
    public void GetDescription_InvalidMemberName_ReturnsEmptyString()
    {
        var description = typeof(SampleEnum).GetDescription("InvalidName");

        description.Should()
                   .BeEmpty();
    }

    [Test]
    public void GetDescription_MemberWithDescription_ReturnsDescriptionText()
    {
        var description = typeof(SampleEnum).GetDescription(SampleEnum.WithDescription);

        description.Should()
                   .Be("This is a sample description");
    }

    [Test]
    public void GetDescription_MemberWithoutDescription_ReturnsEmptyString()
    {
        var description = typeof(SampleEnum).GetDescription(SampleEnum.WithoutDescription);

        description.Should()
                   .BeEmpty();
    }

    //formatter:off
    [Test]
    [Arguments(0, "result")]
    [Arguments(42, "another-result")]
    public void TryCatch_Generic_Returns_Func_Result(int input, string expected)

        //formatter:on
    {
        var result = input.TryCatch<int, string>(_ => expected, _ => "should-not-be-called");

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(1, "handled")]
    [Arguments(2, "handled-2")]
    public void TryCatch_Generic_When_Exception_Returns_Handler_Result(int input, string handlerResult)

        //formatter:on
    {
        string? capturedMessage = null;

        var result = input.TryCatch<int, string>(
            _ => throw new InvalidOperationException("boom"),
            ex =>
            {
                capturedMessage = ex.Message;

                return handlerResult;
            });

        result.Should()
              .Be(handlerResult);

        capturedMessage.Should()
                       .Be("boom");
    }

    [Test]
    public void TryCatch_Void_Executes_Action_When_No_Exception()
    {
        var executed = false;

        0.TryCatch(() => executed = true, _ => throw new Exception("should-not-be-called"));

        executed.Should()
                .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments("boom")]
    [Arguments("oops")]
    public void TryCatch_Void_When_Exception_Calls_Handler(string message)

        //formatter:on
    {
        Exception? captured = null;

        0.TryCatch(() => throw new InvalidOperationException(message), ex => captured = ex);

        captured.Should()
                .NotBeNull();

        captured!.Message
                 .Should()
                 .Be(message);
    }

    private enum SampleEnum
    {
        [Description("This is a sample description")]
        WithDescription,
        WithoutDescription
    }
}