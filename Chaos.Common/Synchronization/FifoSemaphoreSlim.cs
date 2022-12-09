using System.Collections.Concurrent;

namespace Chaos.Common.Synchronization;

/// <summary>
///     The same as <see cref="System.Threading.SemaphoreSlim" />, except with first-in-first-out (FIFO) behavior.
/// </summary>
public sealed class FifoSemaphoreSlim
{
    private readonly SemaphoreSlim Sync;
    private readonly ConcurrentQueue<TaskCompletionSource<bool>> TaskQueue = new();

    public FifoSemaphoreSlim(int initialCount) => Sync = new SemaphoreSlim(initialCount);

    public FifoSemaphoreSlim(int initialCount, int maxCount) => Sync = new SemaphoreSlim(initialCount, maxCount);

    /// <summary>
    ///     Attempts to enter the semaphore. Sets the result on the next tcs in the queue,
    ///     freeing the awaiter of the awaiter of the tcs task to do work while the semaphore is held by the caller
    /// </summary>
    private async void AcquireAndPop()
    {
        await Sync.WaitAsync();

        if (TaskQueue.TryDequeue(out var tcs))
            tcs.SetResult(true);
    }

    /// <summary>
    ///     Releases the internal <see cref="System.Threading.SemaphoreSlim" /> once.
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

    /// <summary>
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns>
    ///     A task that will complete with a result of true if the current thread successfully entered the SemaphoreSlim, otherwise with a
    ///     result of false
    /// </returns>
    public async Task<bool> WaitAsync(TimeSpan timeout)
    {
        var waiter = WaitAsync();

        var ret = await Task.WhenAny(waiter, Task.Delay(timeout)) == waiter;

        //if we fail to acquire sync in the time alotted, we will return false
        //however, the waiter is still waiting, and will eventually enter the sync
        //if we dont Release() here like this, it will never release
        if (!ret)
            _ = waiter.ContinueWith(_ => Release());

        return ret;
    }
}