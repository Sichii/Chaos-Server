namespace Chaos.Common.Abstractions;

/// <summary>
///     Defines a pattern of dynamic retreival from an underlying collection
/// </summary>
public interface IScriptVars
{
    /// <summary>
    ///     Determines whether the IScriptVars contains the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key to locate in the IScriptVars
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the IScriptVars contains an element with the specified key, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool ContainsKey(string key);

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key of the value to locate
    /// </param>
    /// <typeparam name="T">
    ///     The type to convert the result to
    /// </typeparam>
    /// <returns>
    ///     The value at the location specified by the key as the type given. If the value can not be located, or the value can
    ///     not be converted to the correct type, then null
    /// </returns>
    T? Get<T>(string key);

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="type">
    ///     The type to convert the resulting value to
    /// </param>
    /// <param name="key">
    ///     The key of the value to locate
    /// </param>
    /// <returns>
    ///     The value at the location specified by the key as the type given. If the value can not be located, or the value can
    ///     not be converted to the correct type, then null
    /// </returns>
    object? Get(Type type, string key);

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key of the value to locate
    /// </param>
    /// <typeparam name="T">
    ///     The type to convert the result to
    /// </typeparam>
    /// <returns>
    ///     The value at the location specified by the key as the type given. If the value can not be located, or the value can
    ///     not be converted to the correct type, then an exception is thrown
    /// </returns>
    T GetRequired<T>(string key);
}