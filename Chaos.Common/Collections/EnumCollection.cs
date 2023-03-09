using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;
using Chaos.Extensions.Common;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
///     A serializable collection of enums
/// </summary>
[JsonConverter(typeof(EnumCollectionConverter))]
public sealed class EnumCollection : IEnumerable<KeyValuePair<Type, Enum>>
{
    private readonly ConcurrentDictionary<Type, Enum> Enums;

    public EnumCollection() => Enums = new ConcurrentDictionary<Type, Enum>();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, Enum>> GetEnumerator() => Enums.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Removes the enum of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the enum to remove</typeparam>
    public void Remove<T>() where T: Enum => Remove(typeof(T));

    /// <summary>
    ///     Removes the enum of the specified type
    /// </summary>
    /// <param name="enumType">The type of the enum to remove</param>
    public void Remove(Type enumType) => Enums.Remove(enumType, out _);

    /// <summary>
    ///     Sets the enum of the specified type
    /// </summary>
    /// <param name="value">The value of the enum</param>
    /// <typeparam name="T">The type of the enum</typeparam>
    /// <exception cref="InvalidOperationException">Enum cant be a flag</exception>
    public void Set<T>(T value) where T: Enum => Set(typeof(T), value);

    /// <summary>
    ///     Sets the enum of the specified type
    /// </summary>
    /// <param name="enumType">The type of the enum</param>
    /// <param name="enumValue">The value of the enum</param>
    /// <exception cref="InvalidOperationException">Enum cant be a flag</exception>
    public void Set(Type enumType, Enum enumValue)
    {
        if (enumType.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {enumType.FullName} is a flag enum. Use the flag collection.");

        Enums[enumType] = enumValue;
    }

    /// <summary>
    ///     Attempts to retreive the value of the enum of the specified type
    /// </summary>
    /// <param name="value">The value of the enum retreived</param>
    /// <typeparam name="T">The type of the enum to retreive</typeparam>
    /// <returns><c>true</c> if an enum with the specified type was found, otherwise <c>false</c></returns>
    /// <exception cref="InvalidOperationException">Enum cant be a flag</exception>
    public bool TryGetValue<T>([MaybeNullWhen(false)] out T value) where T: Enum
    {
        value = default;

        if (!TryGetValue(typeof(T), out var enumValue))
            return false;

        value = (T)enumValue;

        return true;
    }

    /// <summary>
    ///     Attempts to retreive the value of the enum of the specified type
    /// </summary>
    /// <param name="enumType">The type of the enum to retreive</param>
    /// <param name="enumValue">The value of the enum retreived</param>
    /// <returns><c>true</c> if an enum with the specified type was found, otherwise <c>false</c></returns>
    /// <exception cref="InvalidOperationException">Enum cant be a flag</exception>
    public bool TryGetValue(Type enumType, [MaybeNullWhen(false)] out Enum enumValue)
    {
        if (enumType.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {enumType.FullName} is a flag enum. Use the flag collection.");

        return Enums.TryGetValue(enumType, out enumValue);
    }
}