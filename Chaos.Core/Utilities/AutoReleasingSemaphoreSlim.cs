namespace Chaos.Core.Utilities;

public class AutoReleasingSemaphoreSlim
{
    public SemaphoreSlim SemaphoreSlim { get; }

    public AutoReleasingSemaphoreSlim(int initialCount, int maxCount) => SemaphoreSlim = new SemaphoreSlim(initialCount, maxCount);

    public async ValueTask<IAsyncDisposable> WaitAsync()
    {
        await SemaphoreSlim.WaitAsync();

        return new AutoReleasingSubscription(SemaphoreSlim);
    }

    private record AutoReleasingSubscription : IAsyncDisposable
    {
        private readonly SemaphoreSlim SemaphoreSlim;
        private int Disposed;

        internal AutoReleasingSubscription(SemaphoreSlim semaphoreSlim) => SemaphoreSlim = semaphoreSlim;

        public ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                SemaphoreSlim.Release();

            return ValueTask.CompletedTask;
        }
    }
}