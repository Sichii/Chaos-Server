namespace Chaos.Common.Synchronization;

/// <summary>
///     An object that offers subscription-style blocking synchronization by abusing the using pattern.
/// </summary>
public class AutoReleasingReaderWriterLockSlim
{
    private readonly ReaderWriterLockSlim Root;

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.IsReadLockHeld" />
    /// </summary>
    public bool IsReadLockHeld => Root.IsReadLockHeld;

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.IsWriteLockHeld" />
    /// </summary>
    public bool IsWriteLockHeld => Root.IsWriteLockHeld;

    /// <summary>
    ///     A disposable object that does nothing when disposed.
    /// </summary>
    public static IDisposable NoOpDisposable => new NoOpDisposable();

    /// <summary>
    ///     Creates a new instance of <see cref="AutoReleasingReaderWriterLockSlim" />.
    /// </summary>
    /// <param name="root">
    ///     An optional existing object whose root to lock
    /// </param>
    public AutoReleasingReaderWriterLockSlim(ReaderWriterLockSlim? root = null) => Root = root ?? new ReaderWriterLockSlim();

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.EnterReadLock()" />. Returns a disposable object that when disposed
    ///     will exit the lock />
    /// </summary>
    public IDisposable EnterReadLock()
    {
        Root.EnterReadLock();

        return new AutoReleasingSubscription(Root, true);
    }

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.EnterUpgradeableReadLock()" />. Returns a disposable object that when
    ///     disposed will exit the lock. You can also upgrade to a write lock via the returned object />
    /// </summary>
    /// <returns>
    /// </returns>
    public IUpgradeableLockSubscription EnterUpgradeableReadLock()
    {
        Root.EnterUpgradeableReadLock();

        return new UpgradeableLockSubscription(Root);
    }

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.EnterWriteLock()" />. Returns a disposable object that when disposed
    ///     will exit the lock />
    /// </summary>
    /// <returns>
    /// </returns>
    public IDisposable EnterWriteLock()
    {
        Root.EnterWriteLock();

        return new AutoReleasingSubscription(Root, false);
    }

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.ExitReadLock()" />.
    /// </summary>
    public void ExitReadLock() => Root.ExitReadLock();

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.ExitUpgradeableReadLock()" />.
    /// </summary>
    public void ExitUpgradeableReadLock() => Root.ExitUpgradeableReadLock();

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.ExitWriteLock()" />.
    /// </summary>
    public void ExitWriteLock() => Root.ExitWriteLock();

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.TryEnterReadLock(TimeSpan)" />. Returns a disposable object that when
    ///     disposed will exit the lock
    /// </summary>
    /// <param name="timeoutMs">
    ///     The maximum amount of time to wait before giving up on entering the lock
    /// </param>
    /// <returns>
    ///     <c>
    ///         null
    ///     </c>
    ///     if we failed to enter the lock, otherwise an <see cref="System.IDisposable" /> object that when disposed will exit
    ///     the lock
    /// </returns>
    public IDisposable? TryEnterReadLock(int timeoutMs)
    {
        if (Root.TryEnterReadLock(timeoutMs))
            return new AutoReleasingSubscription(Root, true);

        return default;
    }

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.TryEnterUpgradeableReadLock(TimeSpan)" />. Returns a disposable object
    ///     that when disposed will exit the lock
    /// </summary>
    /// <param name="timeoutMs">
    ///     The maximum amount of time to wait before giving up on entering the lock
    /// </param>
    /// <returns>
    ///     <c>
    ///         null
    ///     </c>
    ///     if we failed to enter the lock, otherwise an <see cref="System.IDisposable" /> object that when disposed will exit
    ///     the lock
    /// </returns>
    public IDisposable? TryEnterUpgradeableReadLock(int timeoutMs)
    {
        if (Root.TryEnterUpgradeableReadLock(timeoutMs))
            return new AutoReleasingSubscription(Root, true);

        return default;
    }

    /// <summary>
    ///     The same as <see cref="ReaderWriterLockSlim.TryEnterWriteLock(TimeSpan)" />. Returns a disposable object that when
    ///     disposed will exit the lock
    /// </summary>
    /// <param name="timeoutMs">
    ///     The maximum amount of time to wait before giving up on entering the lock
    /// </param>
    /// <returns>
    ///     <c>
    ///         null
    ///     </c>
    ///     if we failed to enter the lock, otherwise an <see cref="System.IDisposable" /> object that when disposed will exit
    ///     the lock
    /// </returns>
    public IDisposable? TryEnterWriteLock(int timeoutMs)
    {
        if (Root.TryEnterWriteLock(timeoutMs))
            return new AutoReleasingSubscription(Root, false);

        return default;
    }

    private sealed record AutoReleasingSubscription : IDisposable
    {
        private readonly bool IsRead;
        private readonly ReaderWriterLockSlim Root;
        private int Disposed;

        internal AutoReleasingSubscription(ReaderWriterLockSlim root, bool isRead)
        {
            Root = root;
            IsRead = isRead;
        }

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
            {
                if (IsRead)
                    Root.ExitReadLock();
                else
                    Root.ExitWriteLock();
            }
        }
    }

    /// <summary>
    ///     A disposable subscription that also allows upgrading to a write lock.
    /// </summary>
    public interface IUpgradeableLockSubscription : IDisposable
    {
        /// <summary>
        ///     Upgrades the lock to a write lock
        /// </summary>
        void UpgradeToWriteLock();
    }

    private sealed record UpgradeableLockSubscription : IUpgradeableLockSubscription
    {
        private readonly ReaderWriterLockSlim Root;
        private int Disposed;
        private bool IsRead = true;

        internal UpgradeableLockSubscription(ReaderWriterLockSlim root) => Root = root;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
            {
                if (!IsRead)
                    Root.ExitWriteLock();

                Root.ExitUpgradeableReadLock();
            }
        }

        public void UpgradeToWriteLock()
        {
            if (IsRead)
            {
                Root.EnterWriteLock();
                IsRead = false;
            }
        }
    }
}