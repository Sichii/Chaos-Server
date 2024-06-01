using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

/// <summary>
///     An object that offers subscription-style non-blocking FIFO synchronization by abusing the using pattern.
/// </summary>
public sealed class FifoAutoReleasingSemaphoreSlim
{
    private readonly FifoSemaphoreSlim Root;

    /// <summary>
    ///     The name of the semaphore. Defaults to null (unnamed)
    /// </summary>
    public string? Name
    {
        get => Root.Name;
        set => Root.Name = value;
    }

    /// <inheritdoc cref="FifoSemaphoreSlim.CurrentCount" />
    public int CurrentCount => Root.CurrentCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FifoAutoReleasingSemaphoreSlim" /> class.
    /// </summary>
    /// <param name="initialCount">
    ///     The initial count of the semaphore
    /// </param>
    /// <param name="maxCount">
    ///     The max count of the semaphore
    /// </param>
    /// <param name="name">
    ///     The name of the semaphore. Defaults to null. (unnamed)
    /// </param>
    public FifoAutoReleasingSemaphoreSlim(int initialCount, int maxCount, string? name = null)
        => Root = new FifoSemaphoreSlim(initialCount, maxCount, name);

    /// <summary>
    ///     Releases the internal <see cref="Chaos.Common.Synchronization.FifoSemaphoreSlim" />.
    /// </summary>
    public void Release()
    {
        try
        {
            Root.Release();
        } catch
        {
            //ignored
        }
    }

    /// <summary>
    ///     The same as <see cref="Chaos.Common.Synchronization.FifoSemaphoreSlim.WaitAsync()" />. Returns a disposable object
    ///     that when disposed will release the internal <see cref="Chaos.Common.Synchronization.FifoSemaphoreSlim" />.
    /// </summary>
    public async ValueTask<IPolyDisposable> WaitAsync()
    {
        await Root.WaitAsync();

        return new AutoReleasingSubscription(Root);
    }

    /// <summary>
    ///     Asynchronously waits to enter the semaphore with a timeout.
    /// </summary>
    /// <param name="timeout">
    ///     The amount of time to wait before giving up
    /// </param>
    /// <returns>
    ///     If we enter the semaphore before the timeout elapses, this returns a disposable object that when disposed will
    ///     release the semaphore, otherwise null
    /// </returns>
    public async ValueTask<IPolyDisposable?> WaitAsync(TimeSpan timeout)
    {
        if (await Root.WaitAsync(timeout))
            return new AutoReleasingSubscription(Root);

        return null;
    }

    /// <summary>
    ///     Asynchronously waits to enter the semaphore with a timeout.
    /// </summary>
    /// <param name="timeout">
    ///     The amount of time to wait before giving up
    /// </param>
    /// <param name="subscriptionTask">
    ///     A task that, if the semaphore is acquired, will contain a disposable subscription to that semaphore
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the semaphore was successfully acquired before the timeout, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public ValueTask<bool> WaitAsync(TimeSpan timeout, out Task<IPolyDisposable> subscriptionTask)
    {
        var tcs = new TaskCompletionSource<IPolyDisposable>(TaskCreationOptions.RunContinuationsAsynchronously);
        subscriptionTask = tcs.Task;

        return InnerWaitAsync(timeout, tcs);

        async ValueTask<bool> InnerWaitAsync(TimeSpan localTimeout, TaskCompletionSource<IPolyDisposable> localTcs)
        {
            if (await Root.WaitAsync(localTimeout))
            {
                localTcs.TrySetResult(new AutoReleasingSubscription(Root));

                return true;
            }

            return false;
        }
    }

    private sealed record AutoReleasingSubscription : IPolyDisposable
    {
        private readonly FifoSemaphoreSlim SemaphoreSlim;
        private int Disposed;

        internal AutoReleasingSubscription(FifoSemaphoreSlim semaphoreSlim) => SemaphoreSlim = semaphoreSlim;

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                try
                {
                    SemaphoreSlim.Release();
                } catch
                {
                    //ignored
                }
        }

        public ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                try
                {
                    SemaphoreSlim.Release();
                } catch
                {
                    //ignored
                }

            return ValueTask.CompletedTask;
        }
    }
}