namespace Chaos.Networking.Extensions;

public static class DictionaryExtensions
{
    public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, [MaybeNullWhen(false)] out TValue value) =>
        dic.TryGetValue(key, out value) && dic.Remove(key);
}