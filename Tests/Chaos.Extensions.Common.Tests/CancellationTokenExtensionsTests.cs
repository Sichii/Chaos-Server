using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class CancellationTokenExtensionsTests
{
    [Fact]
    public async Task WaitTillCanceled_Should_Wait_Until_Cancellation_Requested()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Act
        var waitingTask = cts.Token.WaitTillCanceled();

        //Assert
        waitingTask.Status
                   .Should()
                   .NotBe(TaskStatus.RanToCompletion, "because the waiting task should not have completed yet");

        cts.Cancel();
        await waitingTask;

        waitingTask.Status
                   .Should()
                   .Be(TaskStatus.RanToCompletion, "because the waiting task should have completed");
    }

    [Fact]
    public async Task WhenAllWithCancellation_Should_Wait_Until_All_Tasks_Completed_Or_Canceled()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Act
        var whenAllTask = cts.Token.WhenAllWithCancellation(CancellationTokenExtensions.WaitTillCanceled);

        // Assert
        whenAllTask.Status
                   .Should()
                   .NotBe(TaskStatus.RanToCompletion, "because the waiting task should not have completed yet");

        cts.Cancel();
        await whenAllTask;

        whenAllTask.Status
                   .Should()
                   .Be(TaskStatus.RanToCompletion, "because the waiting task should have completed");
    }
}