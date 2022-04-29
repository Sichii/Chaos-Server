namespace Chaos.Core.Utilities;

/// <summary>
///     An object used to control the rate of execution.
/// </summary>
public class RateController : IDisposable
{
    private readonly SemaphoreSlim AutoResetEvent;
    private readonly int DelayMs;
    private readonly int MaxPendingExecutions;
    private int _running;
    public bool Running => _running == 1;

    public RateController(int executionsPerSecond, int maxPendingExecutions)
    {
        _running = 0;
        MaxPendingExecutions = maxPendingExecutions;
        AutoResetEvent = new SemaphoreSlim(1, MaxPendingExecutions);
        DelayMs = 1000 / executionsPerSecond;
    }

    public void Dispose()
    {
        _running = 0;
        GC.SuppressFinalize(this);
        AutoResetEvent.Dispose();
    }

    private async void StartAutoResetEvent()
    {
        try
        {
            while (Running)
            {
                if (AutoResetEvent.CurrentCount < MaxPendingExecutions)
                    AutoResetEvent.Release();

                await Task.Delay(DelayMs).ConfigureAwait(false);
            }
        } catch (ObjectDisposedException)
        {
            //ignore
        }
    }

    public async Task ThrottleAsync()
    {
        if (!Running)
        {
            var doubleCheck = Interlocked.CompareExchange(ref _running, 1, 0) == 1;

            if (!doubleCheck)
                StartAutoResetEvent();
        }

        await AutoResetEvent.WaitAsync().ConfigureAwait(false);
    }
}