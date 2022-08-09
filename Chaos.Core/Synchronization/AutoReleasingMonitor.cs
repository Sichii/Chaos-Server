namespace Chaos.Core.Synchronization;

/// <summary>
///     An object that offers subscription-style blocking synchronization by abusing the using pattern.
/// </summary>
public class AutoReleasingMonitor
{
    private readonly object Root;

    /// <summary>
    ///     The same as <see cref="Monitor.IsEntered(object)" />
    /// </summary>
    public bool IsEntered => Monitor.IsEntered(Root);

    public static IDisposable NoOpDisposable => new NoOpSubscription();

    public AutoReleasingMonitor(object? root = null) => Root = root ?? new object();

    /// <summary>
    ///     The same as <see cref="Monitor.Enter(object)" />.
    ///     Returns a disposable object that when disposed will exit the lock via <see cref="Monitor.Exit(object)" />
    /// </summary>
    public IDisposable Enter()
    {
        Monitor.Enter(Root);

        return new AutoReleasingSubscription(Root);
    }

    /// <summary>
    ///     The same as <see cref="Monitor.Enter(object)" />.
    ///     Returns a disposable object that when disposed will exit the lock via <see cref="Monitor.Exit(object)" />.
    ///     Will first check if the current thread owns the lock in order to avoid an exception.
    /// </summary>
    public IDisposable EnterWithSafeExit()
    {
        Monitor.Enter(Root);

        return new SafeAutoReleasingSubscription(Root);
    }

    /// <summary>
    ///     The same as <see cref="Monitor.Exit(object)" />
    /// </summary>
    public void Exit() => Monitor.Exit(Root);

    /// <summary>
    ///     The same as <see cref="Monitor.TryEnter(object, TimeSpan)" />.
    ///     Returns a disposable object that when disposed will exit the lock via <see cref="Monitor.Exit(object)" />.
    /// </summary>
    /// <param name="timeoutMs"></param>
    /// <returns>
    ///     <c>null</c> if we failed to enter the lock, otherwise an <see cref="IDisposable" /> object that when disposed
    ///     will exit the lock
    /// </returns>
    public IDisposable? TryEnter(int timeoutMs)
    {
        if (Monitor.TryEnter(Root, timeoutMs))
            return new SafeAutoReleasingSubscription(Root);

        return default;
    }

    private record NoOpSubscription : IDisposable
    {
        public void Dispose() { }
    }

    private record SafeAutoReleasingSubscription : IDisposable
    {
        private readonly object Root;
        private int Disposed;

        internal SafeAutoReleasingSubscription(object root) => Root = root;

        public void Dispose()
        {
            if ((Interlocked.CompareExchange(ref Disposed, 1, 0) == 0) && Monitor.IsEntered(Root))
                Monitor.Exit(Root);
        }
    }

    private record AutoReleasingSubscription : IDisposable
    {
        private readonly object Root;
        private int Disposed;

        internal AutoReleasingSubscription(object root) => Root = root;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                Monitor.Exit(Root);
        }
    }
}