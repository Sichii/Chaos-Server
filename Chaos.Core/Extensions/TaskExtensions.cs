namespace Chaos.Core.Extensions;

public static class TaskExtensions
{
    public static async Task WaitTillCanceled(this CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(-1, cancellationToken);
        } catch (TaskCanceledException e)
        {
            //ignored
        }
    }
}