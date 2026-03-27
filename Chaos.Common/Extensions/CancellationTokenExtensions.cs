// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extensions methods for <see cref="System.Threading.CancellationToken" />.
/// </summary>
public static class CancellationTokenExtensions
{
    /// <summary>
    ///     Provides extensions methods for <see cref="System.Threading.CancellationToken" />.
    /// </summary>
    extension(CancellationToken cancellationToken)
    {
        /// <summary>
        ///     Asynchronously waits until cancellation is requested.
        /// </summary>
        public async Task WaitTillCanceled()
        {
            try
            {
                await Task.Delay(-1, cancellationToken);
            } catch (OperationCanceledException)
            {
                //ignored
            }
        }

        /// <summary>
        ///     Asynchronously waits until all tasks are completed or canceled
        /// </summary>
        public Task WhenAllWithCancellation(params IEnumerable<Func<CancellationToken, Task>> taskFuncs)
            => Task.WhenAll(taskFuncs.Select(task => task(cancellationToken)));
    }
}