using Chaos.Common.Abstractions;
using Chaos.Common.Synchronization;

namespace Chaos.Common.Utilities;

public static class ComplexSynchronizationHelper
{
    public static async Task<IPolyDisposable> WaitAsync(
        TimeSpan overallTimeout,
        TimeSpan individualTimeout,
        params FifoAutoReleasingSemaphoreSlim[] synchronizers
    )
    {
        synchronizers = synchronizers.Distinct()
                                     .ToArray();

        var now = DateTime.UtcNow;
        var attemptSignature = new int[synchronizers.Length];

        while (DateTime.UtcNow.Subtract(now) < overallTimeout)
        {
            var disposables = await Task.WhenAll(synchronizers.Select(async sync => await sync.WaitAsync(individualTimeout)));

            if (disposables.Any(disposable => disposable is null))
                for (var i = 0; i < disposables.Length; i++)
                {
                    var disposable = disposables[i];

                    if (disposable is null)
                        attemptSignature[i]++;
                    else
                        disposable.Dispose();
                }
            else
                return new CompositePolyDisposable(disposables!);
        }

        var attempts = attemptSignature.Sum();
        var signature = string.Join(", ", attemptSignature);

        throw new TimeoutException(
            $"The timeout period elapsed. The helper was unable to acquire all semaphores in the alotted time. (Attempts: {attempts
            }, Signature: \"{signature}\")");
    }

    private sealed class CompositePolyDisposable : IPolyDisposable
    {
        private readonly List<IPolyDisposable> Dispoables;

        public CompositePolyDisposable(params IPolyDisposable[] disposables) => Dispoables = disposables.ToList();

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var disposable in Dispoables)
                try
                {
                    disposable.Dispose();
                } catch
                {
                    //ignored
                }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            foreach (var disposable in Dispoables)
                try
                {
                    await disposable.DisposeAsync();
                } catch
                {
                    //ignored
                }
        }
    }
}