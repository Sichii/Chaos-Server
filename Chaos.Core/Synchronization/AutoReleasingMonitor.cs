namespace Chaos.Core.Synchronization;

public class AutoReleasingMonitor
{
    private readonly object Root;

    public bool IsEntered => Monitor.IsEntered(Root);

    public static IDisposable NoOpDisposable => new NoOpSubscription();

    public AutoReleasingMonitor(object? root = null) => Root = root ?? new object();

    public IDisposable Enter()
    {
        Monitor.Enter(Root);

        return new AutoReleasingSubscription(Root);
    }

    public IDisposable EnterWithSafeExit()
    {
        Monitor.Enter(Root);

        return new SafeAutoReleasingSubscription(Root);
    }

    public void Exit() => Monitor.Exit(Root);

    public IDisposable? TryEnter(int timeoutMs)
    {
        if (Monitor.TryEnter(Root, timeoutMs))
            return new AutoReleasingSubscription(Root);

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