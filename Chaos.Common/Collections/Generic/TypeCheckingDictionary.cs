using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Chaos.Common.Collections.Generic;

/// <summary>
///     A wrapper class around a <see cref="System.Collections.Generic.Dictionary{TKey, TValue}" /> that makes it easier to store and retreive polymorphic objects
/// </summary>
/// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}" />
public class TypeCheckingDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey: notnull
{
    private readonly Dictionary<TKey, TValue> Dictionary;

    public TypeCheckingDictionary(IEqualityComparer<TKey>? comparer = null)
    {
        comparer ??= EqualityComparer<TKey>.Default;
        Dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    /// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}.Add" />
    public void Add(TKey key, TValue value) => Dictionary.Add(key, value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}.Remove(TKey)" />
    public bool Remove(TKey key) => Dictionary.Remove(key);

    /// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}.TryAdd" />
    public bool TryAdd(TKey key, TValue value) => Dictionary.TryAdd(key, value);

    /// <typeparam name="T">
    ///     Convert the out parameter to this type. If conversion is not possible, this methid will return
    ///     false.
    /// </typeparam>
    /// <inheritdoc cref="System.Collections.Generic.Dictionary{TKey, TValue}.TryGetValue" />
    public bool TryGetValue<T>(TKey key, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        if (Dictionary.TryGetValue(key, out var untyped))
            if (untyped is T typed)
            {
                value = typed;

                return true;
            }

        return false;
    }

    /// <summary>
    ///     Gets all values of the specified type.
    /// </summary>
    public IEnumerable<T> Values<T>() => Dictionary.Values.OfType<T>();
}