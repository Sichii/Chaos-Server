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
        private int Disposed = 0;
        private readonly object Root;

        internal AutoReleasingSubscription(object root) => Root = root;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref Disposed, 1, 0) == 0)
                Monitor.Exit(Root);
        }
    }
}