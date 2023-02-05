namespace Chaos.Containers;

public class CounterTracker : IEnumerable<KeyValuePair<string, int>>
{
    private readonly ConcurrentDictionary<string, int> Counters;

    public CounterTracker(IEnumerable<KeyValuePair<string, int>>? enumerable = null) =>
        Counters = new ConcurrentDictionary<string, int>(
            enumerable ?? Array.Empty<KeyValuePair<string, int>>(),
            StringComparer.OrdinalIgnoreCase);

    public void AddOrIncrement(string key) => Counters.AddOrUpdate(key, 1, (_, count) => count + 1);

    public void AddOrIncrement(string key, int value) => Counters.AddOrUpdate(key, value, (_, count) => count + value);

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