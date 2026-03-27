#region
using System.Diagnostics.CodeAnalysis;
#endregion

namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines a pattern for an object that caches other objects by key
/// </summary>
/// <typeparam name="TResult">
/// </typeparam>
public interface ISimpleCache<TResult> : IEnumerable<TResult>
{
    /// <summary>
    ///     Determines whether an object with the specified key exists in the cache
    /// </summary>
    /// <param name="key">
    ///     The key of the object to check for existence
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object with the specified key exists in the cache, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Exists(string key);

    /// <summary>
    ///     Forcefully loads all objects into the cache
    /// </summary>
    void ForceLoad();

    /// <summary>
    ///     Gets an object from the cache
    /// </summary>
    /// <param name="key">
    ///     The key to use to find the object
    /// </param>
    TResult Get(string key);

    /// <summary>
    ///     Reloads the cache
    /// </summary>
    Task ReloadAsync();

    /// <summary>
    ///     Reloads a specific entry in the cache
    /// </summary>
    Task ReloadAsync(string key);

    /// <summary>
    ///     Attempts to retrieve an object associated with the specified key from the cache
    /// </summary>
    /// <param name="key">
    ///     The key of the object to retrieve
    /// </param>
    /// <param name="value">
    ///     When this method returns, contains the object retrieved from the cache, if the key exists; otherwise, the default
    ///     value for the type of the object
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object with the specified key exists in the cache, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetValue(string key, [MaybeNullWhen(false)] out TResult value);
}

/// <summary>
///     Defines a pattern for an object that fetches objects of various types from a cache
/// </summary>
public interface ISimpleCache
{
    /// <summary>
    ///     Determines whether an object with the specified key exists in the cache
    /// </summary>
    /// <param name="key">
    ///     The key of the object to check for existence
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object with the specified key exists in the cache, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Exists<TResult>(string key);

    /// <summary>
    ///     Gets an object from a cache
    /// </summary>
    /// <param name="key">
    ///     The key used to find the object
    /// </param>
    /// <typeparam name="TResult">
    ///     The type of object to retreive
    /// </typeparam>
    TResult Get<TResult>(string key);

    /// <summary>
    ///     Reloads the cache of the given type
    /// </summary>
    Task ReloadAsync<T>();

    /// <summary>
    ///     Reloads a specific entry in the cache
    /// </summary>
    Task ReloadAsync<T>(string key);

    /// <summary>
    ///     Attempts to retrieve the value associated with the specified key from the cache
    /// </summary>
    /// <param name="key">
    ///     The key associated with the value to retrieve
    /// </param>
    /// <param name="value">
    ///     When this method returns, contains the value associated with the specified key if it exists, or the default value
    ///     for the type if it does not
    /// </param>
    /// <typeparam name="TResult">
    ///     The type of the value to retrieve
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the value associated with the specified key exists in the cache, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetValue<TResult>(string key, [MaybeNullWhen(false)] out TResult value);
}