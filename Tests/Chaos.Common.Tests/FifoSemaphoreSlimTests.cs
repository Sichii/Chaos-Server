#region
using Chaos.Common.Synchronization;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class FifoSemaphoreSlimTests
{
    [Test]
    public async Task WaitAsync_ShouldAcquireSemaphoreInOrder()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(
                          async index =>
                          {
                              await semaphore.WaitAsync();

                              try
                              {
                                  await Task.Delay(25);

                                  value = index;
                              } finally
                              {
                                  semaphore.Release();
                              }
                          }));

        // Assert
        value.Should()
             .Be(10);
    }

    [Test]
    public async Task WaitAsync_WithTimeout_ShouldAcquireSemaphoreInOrderWithinTimeout()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(
                          async index =>
                          {
                              await semaphore.WaitAsync(TimeSpan.FromMilliseconds(500));

                              try
                              {
                                  await Task.Delay(25);

                                  value = index;
                              } finally
                              {
                                  semaphore.Release();
                              }
                          }));

        // Assert
        value.Should()
             .Be(10);
    }

    [Test]
    public async Task WaitAsync_WithTimeout_ShouldNotAcquireSemaphoreIfTimeoutExpires()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoSemaphoreSlim(1, 1);

        // Act
        var tasks = Enumerable.Range(1, 10)
                              .Select(
                                  async index =>
                                  {
                                      if (!await semaphore.WaitAsync(TimeSpan.FromMilliseconds(450)))
                                          return;

                                      try
                                      {
                                          await Task.Delay(100);

                                          value = index;
                                      } finally
                                      {
                                          semaphore.Release();
                                      }
                                  });

        await Task.WhenAll(tasks);

        // Assert
        value.Should()
             .Be(5, "because there is only enough time in the timeout for entrances to be at (0, 100, 200, 300, 400) milliseconds");
    }
}