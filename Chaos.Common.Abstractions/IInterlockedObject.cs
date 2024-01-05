namespace Chaos.Common.Abstractions;

/// <summary>
///     Defines the pattern for a wrapper around a reference type to perform thread safe operations on it
/// </summary>
public interface IInterlockedObject<T> where T: class
{
    /// <summary>
    ///     Gets the underlying object. Retreival is atomic.
    /// </summary>
    T? Get();

    /// <summary>
    ///     Sets the underlying object. Assignment is atomic.
    /// </summary>
    void Set(T? obj);

    /// <summary>
    ///     Sets the underlying object if the current value is null. Operation is atomic.
    /// </summary>
    /// <param name="obj">
    ///     The object to set to
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the value was null and was set to the provided value, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool SetIfNull(T obj);

    /// <summary>
    ///     Sets the underlying object to null if the caller provides the current value. Operation is atomic.
    /// </summary>
    /// <param name="old">
    ///     The current value
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the value provided was correct and was set to null, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryRemove(T old);

    /// <summary>
    ///     Sets the underlying object to the new value if the old value is provided. Operation is atomic.
    /// </summary>
    /// <param name="new">
    ///     The value to set
    /// </param>
    /// <param name="old">
    ///     The current value
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the value provided was correct and was set to the new value, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryReplace(T @new, T old);
}