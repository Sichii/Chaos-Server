namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extensions methods for <see cref="System.Threading.CancellationToken" />.
/// </summary>
public static class CancellationTokenExtensions
{
    /// <summary>
    ///     Asynchronously waits until cancellation is requested.
    /// </summary>
    public static async Task WaitTillCanceled(this CancellationToken cancellationToken)
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
    public static Task WhenAllWithCancellation(this CancellationToken token, params IEnumerable<Func<CancellationToken, Task>> taskFuncs)
        => Task.WhenAll(taskFuncs.Select(task => task(token)));
}