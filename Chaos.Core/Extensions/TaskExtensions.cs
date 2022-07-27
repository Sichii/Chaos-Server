namespace Chaos.Core.Extensions;

public static class TaskExtensions
{
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

    public static Task WhenAllWithCancellation(this CancellationToken token, params Func<CancellationToken, Task>[] taskFuncs) =>
        Task.WhenAll(taskFuncs.Select(task => task(token)));
}