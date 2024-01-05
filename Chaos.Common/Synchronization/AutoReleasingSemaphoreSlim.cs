namespace Chaos.Common.Synchronization;

/// <summary>
///     An object that offers subscription-style non-blocking synchronization by abusing the using pattern.
/// </summary>
public sealed class AutoReleasingSemaphoreSlim
{
    private readonly SemaphoreSlim Root;

    /// <inheritdoc cref="SemaphoreSlim.CurrentCount" />
    public int CurrentCount => Root.CurrentCount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AutoReleasingSemaphoreSlim" /> class.
    /// </summary>
    /// <param name="initialCount">
    ///     The initial count of the semaphore
    /// </param>
    /// <param name="maxCount">
    ///     The max count of the semaphore
    /// </param>
    public AutoReleasingSemaphoreSlim(int initialCount, int maxCount) => Root = new SemaphoreSlim(initialCount, maxCount);

    /// <summary>
    ///     The same as <see cref="System.Threading.SemaphoreSlim.WaitAsync()" />. Returns a disposable object that when
    ///     disposed will release the internal <see cref="System.Threading.SemaphoreSlim" />.
    /// </summary>
    public async Task<IAsyncDisposable> WaitAsync()
    {
        await Root.WaitAsync();

        return new AutoReleasingSubscription(Root);
    }

    private sealed record AutoReleasingSubscription : IAsyncDisposable
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