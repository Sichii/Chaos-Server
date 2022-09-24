using System.Collections.Concurrent;

namespace Chaos.Common.Synchronization;

/// <summary>
///     The same as <see cref="SemaphoreSlim" />, except with first-in-first-out (FIFO) behavior.
/// </summary>
public class FifoSemaphoreSlim
{
    private readonly SemaphoreSlim Sync;
    private readonly ConcurrentQueue<TaskCompletionSource<bool>> TaskQueue = new();

    public FifoSemaphoreSlim(int initialCount) => Sync = new SemaphoreSlim(initialCount);

    public FifoSemaphoreSlim(int initialCount, int maxCount) => Sync = new SemaphoreSlim(initialCount, maxCount);

    /// <summary>
    ///     Attempts to enter the semaphore. Sets the result on the next tcs in the queue,
    ///     freeing the awaiter of the tcs task to do work inside the semaphore waitAsync
    /// </summary>
    private async void AcquireAndPop()
    {
        await Sync.WaitAsync();

        if (TaskQueue.TryDequeue(out var tcs))
            tcs.SetResult(true);
    }

    /// <summary>
    ///     Releases the internal <see cref="SemaphoreSlim" /> once.
    /// </summary>
    public void Release() => Sync.Release();

    /// <summary>
    ///     Asynchronously waits for a signal to continue.
    /// </summary>
    public Task WaitAsync()
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskQueue.Enqueue(tcs);
        AcquireAndPop();

        return tcs.Task;
    }
}