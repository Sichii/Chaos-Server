using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
///     Represents a thread-safe collection of counters with string keys.
/// </summary>
[JsonConverter(typeof(CounterCollectionConverter))]
public sealed class CounterCollection : IEnumerable<KeyValuePair<string, int>>
{
    private readonly ConcurrentDictionary<string, int> Counters;

    /// <summary>
    ///     Initializes a new instance of the CounterCollection class with an optional initial set of key-value pairs.
    /// </summary>
    public CounterCollection(IEnumerable<KeyValuePair<string, int>>? enumerable = null)
        => Counters = new ConcurrentDictionary<string, int>(
            enumerable ?? Array.Empty<KeyValuePair<string, int>>(),
            StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => Counters.GetEnumerator();

    /// <summary>
    ///     Adds a new counter with the specified key and an initial value of 1, or increments the existing counter by 1.
    /// </summary>
    public int AddOrIncrement(string key) => Counters.AddOrUpdate(key, 1, (_, count) => count + 1);

    /// <summary>
    ///     Adds a new counter with the specified key and the specified initial value, or increments the existing counter by
    ///     the specified value.
    /// </summary>
    public int AddOrIncrement(string key, int value) => Counters.AddOrUpdate(key, value, (_, count) => count + value);

    /// <summary>
    ///     Determines whether the collection contains the specified key.
    /// </summary>
    public bool ContainsKey(string key) => Counters.ContainsKey(key);

    /// <summary>
    ///     Determines whether the counter value associated with the specified key is greater than or equal to the specified
    ///     value.
    /// </summary>
    public bool CounterGreaterThanOrEqualTo(string key, int value) => Counters.TryGetValue(key, out var count) && (count >= value);

    /// <summary>
    ///     Determines whether the counter value associated with the specified key is less than or equal to the specified
    ///     value.
    /// </summary>
    public bool CounterLessThanOrEqualTo(string key, int value) => Counters.TryGetValue(key, out var count) && (count <= value);

    /// <summary>
    ///     Removes the counter with the specified key from the collection and returns its value.
    /// </summary>
    public void Remove(string key, out int value) => Counters.Remove(key, out value);

    /// <summary>
    ///     Sets the counter value associated with the specified key.
    /// </summary>
    public void Set(string key, int value) => Counters[key] = value;

    /// <summary>
    ///     Attempts to decrement the counter value associated with the specified key by 1.
    /// </summary>
    public bool TryDecrement(string key, out int newValue)
    {
        newValue = 0;

        while (true)
        {
            if (!Counters.TryGetValue(key, out var count))
                return false;

            newValue = count - 1;

            if (newValue < 0)
                return false;

            if (Counters.TryUpdate(key, newValue, count))
                return true;
        }
    }

    /// <summary>
    ///     Attempts to decrement the counter value associated with the specified key by the specified value.
    /// </summary>
    public bool TryDecrement(string key, int value, out int newValue)
    {
        newValue = 0;

        while (true)
        {
            if (!Counters.TryGetValue(key, out var count))
                return false;

            newValue = count - value;

            if (newValue < 0)
                return false;

            if (Counters.TryUpdate(key, newValue, count))
                return true;
        }
    }

    /// <summary>
    ///     Attempts to get the counter value associated with the specified key.
    /// </summary>
    public bool TryGetValue(string key, out int value) => Counters.TryGetValue(key, out value);
}