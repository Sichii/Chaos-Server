namespace Chaos.Extensions.Common;

/// <summary>
///     Provides a set of extension methods for <see cref="Action" /> instances.
/// </summary>
public static class ActionExtensions
{
    /// <summary>
    ///     Executes the specified <paramref name="action" /> and catches any exceptions that occur. (Does not rethrow, should
    ///     only be used as a shorthand for unimportant things)
    /// </summary>
    public static void Catch(this Action action, Action<Exception> exceptionHandler)
    {
        try
        {
            action();
        } catch (Exception ex)
        {
            exceptionHandler(ex);
        }
    }
}