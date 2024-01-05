using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;
using Chaos.Common.Synchronization;

namespace Chaos.Common.Utilities;

/// <summary>
///     A helper class for synchronizing multiple semaphores at once
/// </summary>
public static class ComplexSynchronizationHelper
{
    /// <summary>
    ///     Waits for all of the provided semaphores to be available, or for the overall timeout to be reached
    /// </summary>
    /// <param name="overallTimeout">
    ///     The overall timeout for synchronizing all given semaphores
    /// </param>
    /// <param name="individualTimeout">
    ///     The timeout of each attempt to enter a semaphore
    /// </param>
    /// <param name="synchronizers">
    ///     One or more semaphores to be synchronized
    /// </param>
    /// <returns>
    ///     An object that when disposed will release all of the semaphores that were entered
    /// </returns>
    /// <exception cref="TimeoutException">
    ///     The timeout period elapsed. The helper was unable to acquire all semaphores in the alotted time.
    /// </exception>
    /// <remarks>
    ///     If a TimeoutException is thrown, the signature is the number of failed attempts to enter each semaphore, arranged
    ///     in the same order the semaphores were provided to the method
    /// </remarks>
    public static async Task<IPolyDisposable> WaitAsync(
        TimeSpan overallTimeout,
        TimeSpan individualTimeout,
        params FifoAutoReleasingSemaphoreSlim[] synchronizers)
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

    [ExcludeFromCodeCoverage(Justification = "Just a wrapper class")]
    private sealed class CompositePolyDisposable(params IPolyDisposable[] disposables) : IPolyDisposable
    {
        private readonly List<IPolyDisposable> Dispoables = disposables.ToList();

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