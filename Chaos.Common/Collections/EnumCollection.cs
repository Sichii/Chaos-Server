using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

[JsonConverter(typeof(EnumCollectionConverter))]
public sealed class EnumCollection : IEnumerable<KeyValuePair<Type, Enum>>
{
    private readonly ConcurrentDictionary<Type, Enum> Enums;

    public EnumCollection() => Enums = new ConcurrentDictionary<Type, Enum>();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, Enum>> GetEnumerator() => Enums.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Remove<T>() where T: Enum => Remove(typeof(T));

    public void Remove(Type enumType) => Enums.Remove(enumType, out _);

    public void Set<T>(T value) where T: Enum => Set(typeof(T), value);

    public void Set(Type enumType, Enum enumValue) => Enums[enumType] = enumValue;

    public bool TryGetValue<T>([MaybeNullWhen(false)] out T value) where T: Enum
    {
        value = default;

        if (!TryGetValue(typeof(T), out var enumValue))
            return false;

        value = (T)enumValue;

        return true;
    }

    public bool TryGetValue(Type enumType, [MaybeNullWhen(false)] out Enum enumValue) => Enums.TryGetValue(enumType, out enumValue);
}