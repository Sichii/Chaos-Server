using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

[JsonConverter(typeof(CounterCollectionConverter))]
public sealed class CounterCollection : IEnumerable<KeyValuePair<string, int>>
{
    private readonly ConcurrentDictionary<string, int> Counters;

    public CounterCollection(IEnumerable<KeyValuePair<string, int>>? enumerable = null) =>
        Counters = new ConcurrentDictionary<string, int>(
            enumerable ?? Array.Empty<KeyValuePair<string, int>>(),
            StringComparer.OrdinalIgnoreCase);

    public int AddOrIncrement(string key) => Counters.AddOrUpdate(key, 1, (_, count) => count + 1);

    public int AddOrIncrement(string key, int value) => Counters.AddOrUpdate(key, value, (_, count) => count + value);

    public bool ContainsKey(string key) => Counters.ContainsKey(key);

    public bool CounterGreaterThanOrEqualTo(string key, int value) => Counters.TryGetValue(key, out var count) && (count >= value);

    public bool CounterLessThanOrEqualTo(string key, int value) => Counters.TryGetValue(key, out var count) && (count <= value);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => Counters.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Remove(string key, out int value) => Counters.Remove(key, out value);

    public void Set(string key, int value) => Counters[key] = value;

    public bool TryDecrement(string key, out int newValue)
    {
        newValue = 0;

        while (true)
        {
            if (!Counters.TryGetValue(key, out var count))
                return false;

            newValue = count - 1;

            if (Counters.TryUpdate(key, newValue, count))
                return true;
        }
    }

    public bool TryDecrement(string key, int value, out int newValue)
    {
        newValue = 0;

        while (true)
        {
            if (!Counters.TryGetValue(key, out var count))
                return false;

            newValue = count - value;

            if (Counters.TryUpdate(key, newValue, count))
                return true;
        }
    }

    public bool TryGetValue(string key, out int value) => Counters.TryGetValue(key, out value);
}