#region
using Chaos.Common.Synchronization;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class FifoAutoReleasingSemaphoreSlimTests
{
    [Test]
    public void Release_Should_Not_Throw_When_Already_Released()
    {
        var sem = new FifoAutoReleasingSemaphoreSlim(1, 1);
        sem.Release();

        sem.Invoking(s => s.Release())
           .Should()
           .NotThrow();
    }

    [Test]
    public async Task WaitAsync_Should_Return_Disposable_That_Releases_On_Dispose()
    {
        var sem = new FifoAutoReleasingSemaphoreSlim(1, 1);

        var sub = await sem.WaitAsync();

        sem.CurrentCount
           .Should()
           .Be(0);

        sub.Dispose();

        sem.CurrentCount
           .Should()
           .Be(1);
    }

    [Test]
    public async Task WaitAsync_With_Timeout_OutParam_Should_Set_Task_On_Success()
    {
        var sem = new FifoAutoReleasingSemaphoreSlim(1, 1);

        var ok = await sem.WaitAsync(TimeSpan.FromMilliseconds(1), out var subTask);

        ok.Should()
          .BeTrue();

        var sub = await subTask;

        sem.CurrentCount
           .Should()
           .Be(0);
        sub.Dispose();

        sem.CurrentCount
           .Should()
           .Be(1);
    }

    [Test]
    public async Task WaitAsync_With_Timeout_Should_Return_Null_On_Timeout()
    {
        var sem = new FifoAutoReleasingSemaphoreSlim(0, 1);
        var sub = await sem.WaitAsync(TimeSpan.FromMilliseconds(1));

        sub.Should()
           .BeNull();
    }
}

// Remove duplicate file-scoped namespace to avoid CS8954; file is already in Chaos.Common.Tests
//formatter:off
public sealed class FifoAutoReleasingSemaphoreSlimExtendedTests
{
    [Test]
    public async Task WaitAsync_ShouldAcquireSemaphoreInOrder()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(async index =>
                      {
                          await using var sync = await semaphore.WaitAsync();

                          await Task.Delay(25);

                          value = index;
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
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(async index =>
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

    [Test]
    public async Task WaitAsync_WithTimeout_ShouldNotAcquireSemaphoreIfTimeoutExpires()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var tasks = Enumerable.Range(1, 10)
                              .Select(async index =>
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
             .BeInRange(
                 4,
                 5,
                 "because there is only enough time in the timeout for entrances to be at (0, 100, 200, 300, 400) milliseconds");
    }

    [Test]
    public async Task WaitAsync_WithTimeout_With_OutTask_ShouldAcquireSemaphoreInOrderWithinTimeout()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        await Task.WhenAll(
            Enumerable.Range(1, 10)
                      .Select(async index =>
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

    [Test]
    public async Task WaitAsync_WithTimeout_With_OutTask_ShouldNotAcquireSemaphoreIfTimeoutExpires()
    {
        // Arrange
        var value = 0;
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var tasks = Enumerable.Range(1, 10)
                              .Select(async index =>
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
             .BeInRange(
                 4,
                 5,
                 "because there is only enough time in the timeout for entrances to be at (0, 100, 200, 300, 400) milliseconds");
    }

    #region Constructor Tests
    [Test]
    public void Constructor_WithValidParameters_ShouldInitializeProperly()
    {
        // Arrange & Act
        var semaphore = new FifoAutoReleasingSemaphoreSlim(2, 5);

        // Assert
        semaphore.CurrentCount
                 .Should()
                 .Be(2);

        semaphore.Name
                 .Should()
                 .BeNull();
    }

    [Test]
    public void Constructor_WithName_ShouldInitializeProperly()
    {
        // Arrange & Act
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 3, "TestSemaphore");

        // Assert
        semaphore.CurrentCount
                 .Should()
                 .Be(1);

        semaphore.Name
                 .Should()
                 .Be("TestSemaphore");
    }

    [Test]
    public void Constructor_WithZeroInitialCount_ShouldInitializeProperly()
    {
        // Arrange & Act
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 1);

        // Assert
        semaphore.CurrentCount
                 .Should()
                 .Be(0);
    }
    #endregion

    #region Property Tests
    [Test]
    public void Name_WhenSet_ShouldReturnSetValue()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        semaphore.Name = "UpdatedName";

        // Assert
        semaphore.Name
                 .Should()
                 .Be("UpdatedName");
    }

    [Test]
    public void Name_WhenSetToNull_ShouldReturnNull()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1, "InitialName");

        // Act
        semaphore.Name = null;

        // Assert
        semaphore.Name
                 .Should()
                 .BeNull();
    }

    [Test]
    public async Task CurrentCount_AfterAcquiring_ShouldDecrease()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(2, 2);
        var initialCount = semaphore.CurrentCount;

        // Act
        var subscription = await semaphore.WaitAsync();
        var countAfterAcquire = semaphore.CurrentCount;

        // Assert
        initialCount.Should()
                    .Be(2);

        countAfterAcquire.Should()
                         .Be(1);

        // Cleanup
        await subscription.DisposeAsync();
    }

    [Test]
    public async Task CurrentCount_AfterReleasing_ShouldIncrease()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 2);
        var subscription = await semaphore.WaitAsync();
        var countAfterAcquire = semaphore.CurrentCount;

        // Act
        await subscription.DisposeAsync();
        var countAfterRelease = semaphore.CurrentCount;

        // Assert
        countAfterAcquire.Should()
                         .Be(0);

        countAfterRelease.Should()
                         .Be(1);
    }
    #endregion

    #region Release Method Tests
    [Test]
    public async Task Release_WhenCalled_ShouldIncreaseCurrentCount()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 2);
        var initialCount = semaphore.CurrentCount;

        // Act
        semaphore.Release();
        await Task.Delay(10); // Small delay to ensure release is processed
        var countAfterRelease = semaphore.CurrentCount;

        // Assert
        initialCount.Should()
                    .Be(0);

        countAfterRelease.Should()
                         .Be(1);
    }

    [Test]
    public void Release_WhenCalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act & Assert
        var act1 = () => semaphore.Release();
        var act2 = () => semaphore.Release();
        var act3 = () => semaphore.Release();

        act1.Should()
            .NotThrow();

        act2.Should()
            .NotThrow();

        act3.Should()
            .NotThrow();
    }
    #endregion

    #region AutoReleasingSubscription Tests
    [Test]
    public async Task AutoReleasingSubscription_WhenDisposedSynchronously_ShouldReleaseOnce()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);
        var subscription = await semaphore.WaitAsync();
        var countBeforeDispose = semaphore.CurrentCount;

        // Act
        subscription.Dispose();
        var countAfterDispose = semaphore.CurrentCount;

        // Assert
        countBeforeDispose.Should()
                          .Be(0);

        countAfterDispose.Should()
                         .Be(1);
    }

    [Test]
    public async Task AutoReleasingSubscription_WhenDisposedAsynchronously_ShouldReleaseOnce()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);
        var subscription = await semaphore.WaitAsync();
        var countBeforeDispose = semaphore.CurrentCount;

        // Act
        await subscription.DisposeAsync();
        var countAfterDispose = semaphore.CurrentCount;

        // Assert
        countBeforeDispose.Should()
                          .Be(0);

        countAfterDispose.Should()
                         .Be(1);
    }

    [Test]
    public async Task AutoReleasingSubscription_WhenDisposedMultipleTimes_ShouldReleaseOnlyOnce()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);
        var subscription = await semaphore.WaitAsync();

        // Act
        subscription.Dispose();
        var countAfterFirstDispose = semaphore.CurrentCount;
        subscription.Dispose();
        var countAfterSecondDispose = semaphore.CurrentCount;
        await subscription.DisposeAsync();
        var countAfterAsyncDispose = semaphore.CurrentCount;

        // Assert
        countAfterFirstDispose.Should()
                              .Be(1);

        countAfterSecondDispose.Should()
                               .Be(1);

        countAfterAsyncDispose.Should()
                              .Be(1);
    }

    [Test]
    public async Task AutoReleasingSubscription_MixedDisposalMethods_ShouldReleaseOnlyOnce()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);
        var subscription = await semaphore.WaitAsync();

        // Act
        subscription.Dispose();
        await subscription.DisposeAsync();
        subscription.Dispose();
        var countAfterAllDisposes = semaphore.CurrentCount;

        // Assert
        countAfterAllDisposes.Should()
                             .Be(1);
    }
    #endregion

    #region Timeout Edge Cases
    [Test]
    public async Task WaitAsync_WithZeroTimeout_ShouldReturnImmediatelyIfAvailable()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var subscription = await semaphore.WaitAsync(TimeSpan.Zero);

        // Assert
        subscription.Should()
                    .NotBeNull();

        // Cleanup
        await subscription.DisposeAsync();
    }

    [Test]
    public async Task WaitAsync_WithZeroTimeout_ShouldReturnNullIfNotAvailable()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 1);

        // Act
        var subscription = await semaphore.WaitAsync(TimeSpan.Zero);

        // Assert
        subscription.Should()
                    .BeNull();
    }

    [Test]
    public async Task WaitAsync_WithNegativeTimeout_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var act = async () => await semaphore.WaitAsync(TimeSpan.FromMilliseconds(-100));

        // Assert
        await act.Should()
                 .ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task WaitAsync_WithOutTask_ZeroTimeout_ShouldReturnFalseIfNotAvailable()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 1);

        // Act
        var result = await semaphore.WaitAsync(TimeSpan.Zero, out var subscriptionTask);

        // Assert
        result.Should()
              .BeFalse();

        subscriptionTask.Should()
                        .NotBeNull();
    }

    [Test]
    public async Task WaitAsync_WithOutTask_ZeroTimeout_ShouldReturnTrueIfAvailable()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);

        // Act
        var result = await semaphore.WaitAsync(TimeSpan.Zero, out var subscriptionTask);

        // Assert
        result.Should()
              .BeTrue();

        subscriptionTask.Should()
                        .NotBeNull();

        // Cleanup
        var subscription = await subscriptionTask;
        await subscription.DisposeAsync();
    }
    #endregion

    #region Thread Safety Tests
    [Test]
    public async Task WaitAsync_ConcurrentAccess_ShouldProcessAllRequests()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(1, 1);
        var results = new List<int>();
        var tasks = new List<Task>();

        // Act - Create tasks that will compete for the semaphore
        for (var i = 0; i < 5; i++)
        {
            var index = i;

            tasks.Add(
                Task.Run(async () =>
                {
                    await using var subscription = await semaphore.WaitAsync();

                    lock (results)
                        results.Add(index);

                    await Task.Delay(10); // Hold the semaphore briefly
                }));
        }

        await Task.WhenAll(tasks);

        // Assert - All tasks should complete (order may vary due to task scheduling)
        results.Should()
               .HaveCount(5);

        results.Should()
               .Contain(
                   new[]
                   {
                       0,
                       1,
                       2,
                       3,
                       4
                   });
    }

    [Test]
    public async Task Release_ConcurrentCalls_ShouldBeThreadSafe()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 10);
        var tasks = new List<Task>();

        // Act
        for (var i = 0; i < 10; i++)
            tasks.Add(Task.Run(() => semaphore.Release()));

        await Task.WhenAll(tasks);
        await Task.Delay(50); // Allow releases to be processed

        // Assert
        semaphore.CurrentCount
                 .Should()
                 .BeInRange(0, 10); // Should not exceed max count
    }
    #endregion

    #region Error Condition Tests
    [Test]
    public async Task WaitAsync_WithLongTimeout_ShouldEventuallyAcquire()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 1);
        var acquireTask = semaphore.WaitAsync(TimeSpan.FromSeconds(1));

        // Act
        await Task.Delay(100); // Let the wait start
        semaphore.Release(); // Release after a short delay
        var subscription = await acquireTask;

        // Assert
        subscription.Should()
                    .NotBeNull();

        // Cleanup
        await subscription.DisposeAsync();
    }

    [Test]
    public async Task WaitAsync_WithOutTask_ShouldCompleteTaskWhenAcquired()
    {
        // Arrange
        var semaphore = new FifoAutoReleasingSemaphoreSlim(0, 1);
        var waitTask = semaphore.WaitAsync(TimeSpan.FromSeconds(1), out var subscriptionTask);

        // Act
        await Task.Delay(100); // Let the wait start
        semaphore.Release(); // Release after a short delay
        var acquired = await waitTask;
        var subscription = await subscriptionTask;

        // Assert
        acquired.Should()
                .BeTrue();

        subscription.Should()
                    .NotBeNull();

        subscriptionTask.IsCompleted
                        .Should()
                        .BeTrue();

        // Cleanup
        await subscription.DisposeAsync();
    }
    #endregion
}