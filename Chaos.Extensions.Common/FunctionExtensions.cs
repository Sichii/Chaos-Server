namespace Chaos.Extensions.Common;

/// <summary>
///     Provides a set of extension methods for <see cref="Func{TResult}" /> instances.
/// </summary>
public static class FunctionExtensions
{
    /// <summary>
    ///     Executes the specified <paramref name="func" /> and catches any exceptions that occur. (Does not rethrow, should
    ///     only be used as a shorthand for unimportant things)
    /// </summary>
    public static T Catch<T>(this Func<T> func, Func<Exception, T> exceptionHandler)
    {
        try
        {
            return func();
        } catch (Exception ex)
        {
            return exceptionHandler(ex);
        }
    }
}