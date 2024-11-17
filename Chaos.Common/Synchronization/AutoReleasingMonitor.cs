namespace Chaos.Common.Synchronization;

/// <summary>
///     An object that offers subscription-style blocking synchronization by abusing the using pattern.
/// </summary>
[Obsolete("Use System.Threading.Lock instead")]
public sealed class AutoReleasingMonitor
{
    private readonly object Root;

    /// <summary>
    ///     The same as <see cref="System.Threading.Monitor.IsEntered(object)" />
    /// </summary>
    public bool IsEntered => Monitor.IsEntered(Root);

    /// <summary>
    ///     A disposable object that does nothing when disposed.
    /// </summary>
    public static IDisposable NoOpDisposable => new NoOpDisposable();

    /// <summary>
    ///     Creates a new instance of <see cref="AutoReleasingMonitor" />.
    /// </summary>
    /// <param name="root">
    ///     An optional existing object whose root to lock
    /// </param>
    public AutoReleasingMonitor(object? root = null) => Root = root ?? new object();

    /// <summary>
    ///     The same as <see cref="System.Threading.Monitor.Enter(object)" />. Returns a disposable object that when disposed
    ///     will exit the lock via <see cref="System.Threading.Monitor.Exit(object)" />
    /// </summary>
    public IDisposable Enter()
    {
        Monitor.Enter(Root);

        return new AutoReleasingSubscription(Root);
    }

    /// <summary>
    ///     The same as <see cref="System.Threading.Monitor.Exit(object)" />
    /// </summary>
    public void Exit() => Monitor.Exit(Root);

    /// <summary>
    ///     The same as <see cref="System.Threading.Monitor.TryEnter(object, TimeSpan)" />. Returns a disposable object that
    ///     when disposed will exit the lock via <see cref="System.Threading.Monitor.Exit(object)" />.
    /// </summary>
    /// <param name="timeoutMs">
    /// </param>
    /// <returns>
    ///     <c>
    ///         null
    ///     </c>
    ///     if we failed to enter the lock, otherwise an <see cref="System.IDisposable" /> object that when disposed will exit
    ///     the lock
    /// </returns>
    public IDisposable? TryEnter(int timeoutMs)
    {
        if (Monitor.TryEnter(Root, timeoutMs))
            return new AutoReleasingSubscription(Root);

        return default;
    }

    private sealed record AutoReleasingSubscription : IDisposable
    {
        private readonly object Root;
        private int Disposed;

        internal AutoReleasingSubscription(object root) => Root = root;

        public void Dispose()
        {
            if ((Interlocked.CompareExchange(ref Disposed, 1, 0) == 0) && Monitor.IsEntered(Root))
                Monitor.Exit(Root);
        }
    }
}