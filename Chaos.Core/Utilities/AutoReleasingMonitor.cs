namespace Chaos.Core.Utilities;

public class AutoReleasingMonitor
{
    private readonly object Root;

    public AutoReleasingMonitor(object? root = null) => Root = root ?? new object();

    public IDisposable Enter()
    {
        Monitor.Enter(Root);

        return new AutoReleasingSubscription(Root);
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