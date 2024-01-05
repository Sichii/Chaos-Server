namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for the <see cref="Stack{T}" /> class.
/// </summary>
public static class StackExtensions
{
    /// <summary>
    ///     Pops items from the top of the stack until the specified <paramref name="predicate" /> returns true.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the stack.
    /// </typeparam>
    /// <param name="stack">
    ///     The stack to pop items from.
    /// </param>
    /// <param name="predicate">
    ///     The condition to test each element against.
    /// </param>
    /// <returns>
    ///     The first item that satisfies the <paramref name="predicate" />, or null if the stack is empty or no item satisfies
    ///     the predicate.
    /// </returns>
    public static T? PopUntil<T>(this Stack<T> stack, Func<T, bool> predicate)
    {
        while (stack.TryPop(out var item))
            if (predicate(item))
                return item;

        return default;
    }
}