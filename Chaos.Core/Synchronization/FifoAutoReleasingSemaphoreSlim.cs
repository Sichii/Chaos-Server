namespace Chaos.Core.Synchronization;

public class FifoAutoReleasingSemaphoreSlim
{
    public FifoSemaphoreSlim SemaphoreSlim { get; }

    public FifoAutoReleasingSemaphoreSlim(int initialCount, int maxCount) => SemaphoreSlim = new FifoSemaphoreSlim(initialCount, maxCount);

    public async ValueTask<IAsyncDisposable> WaitAsync()
    {
        await SemaphoreSlim.WaitAsync();

        return new AutoReleasingSubscription(SemaphoreSlim);
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