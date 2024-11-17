#region
using Chaos.Common.Abstractions;
using Chaos.Common.Synchronization;
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class ComplexSynchronizationHelperTests
{
    [Test]
    public async Task WaitAsync_AllSemaphoresAvailable_ReturnsPolyDisposable()
    {
        // Arrange
        var synchronizers = Enumerable.Range(1, 3)
                                      .Select(_ => new FifoAutoReleasingSemaphoreSlim(1, 1))
                                      .ToArray();

        var overallTimeout = TimeSpan.FromSeconds(5);
        var individualTimeout = TimeSpan.FromSeconds(1);

        // Act
        var result = await ComplexSynchronizationHelper.WaitAsync(overallTimeout, individualTimeout, synchronizers);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .BeAssignableTo<IPolyDisposable>();

        foreach (var synchronizer in synchronizers)
            synchronizer.CurrentCount
                        .Should()
                        .Be(0);
    }

    [Test]
    public async Task WaitAsync_NotAllSemaphoresAvailable_ThrowsTimeoutException()
    {
        // Arrange
        var synchronizers = new[]
        {
            new FifoAutoReleasingSemaphoreSlim(1, 1),
            new FifoAutoReleasingSemaphoreSlim(1, 1),
            new FifoAutoReleasingSemaphoreSlim(0, 1)
        };

        var overallTimeout = TimeSpan.FromSeconds(1);
        var individualTimeout = TimeSpan.FromMilliseconds(100);

        // Act
        Func<Task> action = async () => await ComplexSynchronizationHelper.WaitAsync(overallTimeout, individualTimeout, synchronizers);

        // Assert
        await action.Should()
                    .ThrowAsync<TimeoutException>();
    }
}