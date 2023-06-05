using Xunit;
using Xunit.Abstractions;

namespace Chaos.Tests;

public sealed class ThreadSchedulerTests
{
    private readonly ITestOutputHelper OutputHelper;

    public ThreadSchedulerTests(ITestOutputHelper outputHelper) => OutputHelper = outputHelper;

    [Fact]
    public async Task SchedulerTest()
    {
        async Task RunSomeWorkAsync(string taskName)
        {
            OutputHelper.WriteLine(taskName + ": " + Environment.CurrentManagedThreadId);

            await Task.Delay(1000);

            OutputHelper.WriteLine(taskName + ": " + Environment.CurrentManagedThreadId);

            //simulate work
            Thread.Sleep(5000);
        }

        OutputHelper.WriteLine("parent: " + Environment.CurrentManagedThreadId);

        var task1 = RunSomeWorkAsync("task1");
        var task2 = RunSomeWorkAsync("task2");
        var task3 = RunSomeWorkAsync("task3");

        await Task.WhenAll(task1, task2, task3);

        OutputHelper.WriteLine("parent: " + Environment.CurrentManagedThreadId);
    }
}