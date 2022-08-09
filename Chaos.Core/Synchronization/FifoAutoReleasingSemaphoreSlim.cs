namespace Chaos.Core.Synchronization;

/// <summary>
///     An object that offers subscription-style non-blocking FIFO synchronization by abusing the using pattern.
/// </summary>
public class FifoAutoReleasingSemaphoreSlim
{
    public FifoSemaphoreSlim Root { get; }

    public FifoAutoReleasingSemaphoreSlim(int initialCount, int maxCount) => Root = new FifoSemaphoreSlim(initialCount, maxCount);

    /// <summary>
    ///     The same as <see cref="FifoSemaphoreSlim.WaitAsync()" />.
    ///     Returns a disposable object that when disposed will release the internal <see cref="FifoSemaphoreSlim" />.
    /// </summary>
    public async ValueTask<IAsyncDisposable> WaitAsync()
    {
        await Root.WaitAsync();

        return new AutoReleasingSubscription(Root);
    }

    private record AutoReleasingSubscription : IAsyncDisposable
    {
        private readonly FifoSemaphoreSlim SemaphoreSlim;
        private int Disposed;

        internal AutoReleasingSubscription(FifoSemaphoreSlim semaphoreSlim) => SemaphoreSlim = semaphoreSlim;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                SemaphoreSlim.Release();

            return ValueTask.CompletedTask;
        }
    }
}