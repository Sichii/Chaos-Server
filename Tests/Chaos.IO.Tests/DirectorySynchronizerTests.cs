using System.Diagnostics;
using Chaos.IO.FileSystem;
using FluentAssertions;
using Xunit;

namespace Chaos.IO.Tests;

public sealed class DirectorySynchronizerTests
{
    [Fact]
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

        DirectorySynchronizer.LockedDirectories.Should().Contain(DIRECTORY);
    }

    [Fact]
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

        counter.Should().Be(2);
        Stopwatch.GetElapsedTime(start).Should().BeGreaterThan(TimeSpan.FromMilliseconds(200));
    }

    [Fact]
    public void SafeExecute_ShouldReturnCorrectResult()
    {
        const string DIRECTORY = "testDir";

        var result = DIRECTORY.SafeExecute(_ => "success");

        result.Should().Be("success");
    }

    [Fact]
    public void SafeExecuteAsync_ShouldLockDirectory()
    {
        const string DIRECTORY = "testDir";

        _ = DIRECTORY.SafeExecuteAsync(
            async _ =>
            {
                await Task.Delay(500); // Simulate a long-running operation
            });

        DirectorySynchronizer.LockedDirectories.Should().Contain(DIRECTORY);
    }

    [Fact]
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

        counter.Should().Be(2, "each function should have incremented the counter once");
        end.Should().BeGreaterThan(TimeSpan.FromMilliseconds(200), "the tasks should not have been executed concurrently");
    }

    [Fact]
    public async Task SafeExecuteAsync_ShouldReturnCorrectResult()
    {
        const string DIRECTORY = "testDir";

        var result = await DIRECTORY.SafeExecuteAsync(_ => Task.FromResult("success"));

        result.Should().Be("success");
    }
}