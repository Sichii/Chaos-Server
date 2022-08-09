namespace Chaos.Core.Extensions;

public static class TaskExtensions
{
    /// <summary>
    ///     Asynchronously waits until cancellation is requested.
    /// </summary>
    public static async Task WaitTillCanceled(this CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        } catch (TaskCanceledException)
        {
            //ignored
        }
    }

    /// <summary>
    ///     Asynchronously waits till all tasks are completed or canceled
    /// </summary>
    public static Task WhenAllWithCancellation(this CancellationToken token, params Func<CancellationToken, Task>[] taskFuncs) =>
        Task.WhenAll(taskFuncs.Select(task => task(token)));
}