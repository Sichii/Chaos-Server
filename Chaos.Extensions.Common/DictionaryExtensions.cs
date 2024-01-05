using System.Diagnostics.CodeAnalysis;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Collections.Generic.IDictionary{TKey,TValue}" />
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Removes the element with the specified key from the
    ///     <see cref="System.Collections.Generic.IDictionary{TKey,TValue}" /> and makes the value available through the
    ///     <paramref name="value" /> parameter.
    /// </summary>
    /// <param name="dic">
    ///     The dictionary to remove the element from
    /// </param>
    /// <param name="key">
    ///     The key of the element to remove
    /// </param>
    /// <param name="value">
    ///     When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
    ///     value for the type of the value parameter. This parameter is passed uninitialized
    /// </param>
    /// <typeparam name="TKey">
    ///     The type of the key
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the object that implements <see cref="System.Collections.Generic.IDictionary{TKey,TValue}" /> contains an
    ///     element with the specified key; otherwise,
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, [MaybeNullWhen(false)] out TValue value)
        => dic.TryGetValue(key, out value) && dic.Remove(key);
}