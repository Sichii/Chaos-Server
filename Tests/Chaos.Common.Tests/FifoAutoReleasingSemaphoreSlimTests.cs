using Chaos.Common.Synchronization;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class FifoAutoReleasingSemaphoreSlimTests
{
    [Fact]
    public async Task WaitAsync_ShouldAcquireSemaphoreInOrder()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(
                          async index =>
                          {
                              await using var sync = await semaphore.WaitAsync();

                              await Task.Delay(25);

                              value = index;
                          }));

        // Assert
        value.Should()
             .Be(10);
    }

    [Fact]
    public async Task WaitAsync_WithTimeout_ShouldAcquireSemaphoreInOrderWithinTimeout()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(
                          async index =>
                          {
                              var disposable = await semaphore.WaitAsync(TimeSpan.FromMilliseconds(500));

                              if (disposable is null)
                                  return;

                              await using var sync = disposable;

                              await Task.Delay(25);

                              value = index;
                          }));

        // Assert
        value.Should()
             .Be(10);
    }

    [Fact]
    public async Task WaitAsync_WithTimeout_ShouldNotAcquireSemaphoreIfTimeoutExpires()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var tasks = Enumerable.Range(1, 10)
                              .Select(
                                  async index =>
                                  {
                                      var disposable = await semaphore.WaitAsync(TimeSpan.FromMilliseconds(450));

                                      if (disposable is null)
                                          return;

                                      await using var sync = disposable;

                                      await Task.Delay(100);

                                      value = index;
                                  });

        await Task.WhenAll(tasks);

        // Assert
        value.Should()
             .Be(5, "because there is only enough time in the timeout for entrances to be at (0, 100, 200, 300, 400) milliseconds");
    }

    [Fact]
    public async Task WaitAsync_WithTimeout_With_OutTask_ShouldAcquireSemaphoreInOrderWithinTimeout()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(
                          async index =>
                          {
                              if (!await semaphore.WaitAsync(TimeSpan.FromMilliseconds(500), out var subTask))
                                  return;

                              await using var sync = await subTask;

                              await Task.Delay(25);

                              value = index;
                          }));

        // Assert
        value.Should()
             .Be(10);
    }

    [Fact]
    public async Task WaitAsync_WithTimeout_With_OutTask_ShouldNotAcquireSemaphoreIfTimeoutExpires()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var tasks = Enumerable.Range(1, 10)
                              .Select(
                                  async index =>
                                  {
                                      if (!await semaphore.WaitAsync(TimeSpan.FromMilliseconds(450), out var subTask))
                                          return;

                                      await using var sync = await subTask;

                                      await Task.Delay(100);

                                      value = index;
                                  });

        await Task.WhenAll(tasks);

        // Assert
        value.Should()
             .Be(5, "because there is only enough time in the timeout for entrances to be at (0, 100, 200, 300, 400) milliseconds");
    }
}