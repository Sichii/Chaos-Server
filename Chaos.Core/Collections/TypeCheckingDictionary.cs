namespace Chaos.Core.Collections;

public class TypeCheckingDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey: notnull
{
    private readonly Dictionary<TKey, TValue> Dictionary;

    public TypeCheckingDictionary(IEqualityComparer<TKey>? comparer = null)
    {
        comparer ??= EqualityComparer<TKey>.Default;
        Dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public void Add(TKey key, TValue value) => Dictionary.Add(key, value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dictionary.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Remove(TKey key) => Dictionary.Remove(key);

    public bool TryAdd(TKey key, TValue value) => Dictionary.TryAdd(key, value);

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

    public IEnumerable<T> Values<T>() => Dictionary.Values.OfType<T>();
}