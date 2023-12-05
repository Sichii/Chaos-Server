using Chaos.Common.Synchronization;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class AutoReleasingSemaphoreSlimTests
{
    [Fact]
    public async Task WaitAsync_ShouldAcquireSemaphoreAndReleaseOnDisposeAsync()
    {
        // Arrange
        var semaphore = new AutoReleasingSemaphoreSlim(1, 1);
        bool semaphoreAcquired;

        // Act
        await using (await semaphore.WaitAsync())
            semaphoreAcquired = true;

        // Perform operations inside the semaphore
        // Assert
        semaphoreAcquired.Should()
                         .BeTrue();

        semaphore.CurrentCount
                 .Should()
                 .Be(1);
    }
}