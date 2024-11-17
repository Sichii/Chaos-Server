#region
using System.Diagnostics;
using Chaos.IO.FileSystem;
using FluentAssertions;
#endregion

namespace Chaos.IO.Tests;

[NotInParallel]
public sealed class DirectorySynchronizerTests
{
    [Test]
    public void SafeExecute_ShouldLockDirectory()
    {
        const string DIRECTORY = "testDir";

        _ = Task.Run(
            () => DIRECTORY.SafeExecute(
                _ =>
                {
                    Thread.Sleep(100); // Simulate a long-running operation
                }));

        Thread.Sleep(50);

        DirectorySynchronizer.LockedPaths
                             .Should()
                             .Contain(DIRECTORY);
    }

    [Test]
    public async Task SafeExecute_ShouldNotAllowConcurrentExecution()
    {
        const string DIRECTORY = "testDir";
        var counter = 0;
        var start = Stopwatch.GetTimestamp();

        var t1 = Task.Run(
            () => DIRECTORY.SafeExecute(
                _ =>
                {
                    counter++;
                    Thread.Sleep(100);
                }));

        var t2 = Task.Run(
            () => DIRECTORY.SafeExecute(
                _ =>
                {
                    counter++;
                    Thread.Sleep(100);
                }));

        await Task.WhenAll(t1, t2);

        counter.Should()
               .Be(2);

        Stopwatch.GetElapsedTime(start)
                 .Should()
                 .BeGreaterThan(TimeSpan.FromMilliseconds(200));
    }

    [Test]
    public void SafeExecute_ShouldReturnCorrectResult()
    {
        const string DIRECTORY = "testDir";

        var result = DIRECTORY.SafeExecute(_ => "success");

        result.Should()
              .Be("success");
    }

    [Test]
    public void SafeExecuteAsync_ShouldLockDirectory()
    {
        const string DIRECTORY = "testDir";

        _ = DIRECTORY.SafeExecuteAsync(
            async _ =>
            {
                await Task.Delay(500); // Simulate a long-running operation
            });

        DirectorySynchronizer.LockedPaths
                             .Should()
                             .Contain(DIRECTORY);
    }

    [Test]
    public async Task SafeExecuteAsync_ShouldNotAllowConcurrentExecution()
    {
        const string DIRECTORY = "testDir";
        var counter = 0;
        var start = Stopwatch.GetTimestamp();

        var task1 = DIRECTORY.SafeExecuteAsync(
            async _ =>
            {
                counter++;
                await Task.Delay(100);
            });

        var task2 = DIRECTORY.SafeExecuteAsync(
            async _ =>
            {
                counter++;
                await Task.Delay(100);
            });

        await Task.WhenAll(task1, task2);
        var end = Stopwatch.GetElapsedTime(start);

        counter.Should()
               .Be(2, "each function should have incremented the counter once");

        end.Should()
           .BeGreaterThan(TimeSpan.FromMilliseconds(200), "the tasks should not have been executed concurrently");
    }

    [Test]
    public async Task SafeExecuteAsync_ShouldReturnCorrectResult()
    {
        const string DIRECTORY = "testDir";

        var result = await DIRECTORY.SafeExecuteAsync(_ => Task.FromResult("success"));

        result.Should()
              .Be("success");
    }
}