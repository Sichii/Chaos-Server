using Chaos.Common.Abstractions;

namespace Chaos.Common.Synchronization;

/// <summary>
///     An object that offers subscription-style non-blocking FIFO synchronization by abusing the using pattern.
/// </summary>
public class FifoAutoReleasingSemaphoreSlim
{
    public FifoSemaphoreSlim Root { get; }

    public FifoAutoReleasingSemaphoreSlim(int initialCount, int maxCount) => Root = new FifoSemaphoreSlim(initialCount, maxCount);

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
    ///     The same as <see cref="FifoSemaphoreSlim.WaitAsync()" />.
    ///     Returns a disposable object that when disposed will release the internal <see cref="FifoSemaphoreSlim" />.
    /// </summary>
    public async ValueTask<IPolyDisposable> WaitAsync()
    {
        await Root.WaitAsync();

        return new AutoReleasingSubscription(Root);
    }

    private record AutoReleasingSubscription : IPolyDisposable
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