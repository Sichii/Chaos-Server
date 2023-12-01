using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;
using Chaos.Extensions.Common;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
///     A serializable collection of enum flags
/// </summary>
[JsonConverter(typeof(FlagCollectionConverter))]
public sealed class FlagCollection : IEnumerable<KeyValuePair<Type, Enum>>
{
    private readonly ConcurrentDictionary<Type, Enum> Flags;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FlagCollection" /> class
    /// </summary>
    public FlagCollection() => Flags = new ConcurrentDictionary<Type, Enum>();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, Enum>> GetEnumerator() => Flags.GetEnumerator();

    /// <summary>
    ///     Adds a flag to an existing flag of the same type, or sets it
    /// </summary>
    /// <param name="flag">The flag to add or set</param>
    /// <typeparam name="T">The type of the flag</typeparam>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    public void AddFlag<T>(T flag) where T: Enum => AddFlag(typeof(T), flag);

    /// <summary>
    ///     Adds a flag to an existing flag of the same type, or sets it
    /// </summary>
    /// <param name="flagType">The type of the flag</param>
    /// <param name="flagValue">The flag to add or set</param>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    public void AddFlag(Type flagType, Enum flagValue)
    {
        if (!flagType.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {flagType.FullName} is not a flag enum. Use the enum collection.");

        if (Flags.TryGetValue(flagType, out var value))
            Flags[flagType] = (Enum)Enum.ToObject(flagType, Convert.ToUInt64(value) | Convert.ToUInt64(flagValue));
        else
            Flags.TryAdd(flagType, flagValue);
    }

    /// <summary>
    ///     Gets the flag of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the flag to retreive</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    /// <exception cref="KeyNotFoundException">Flag type not found</exception>
    public T GetFlag<T>() where T: Enum
    {
        var flagType = typeof(T);

        if (!flagType.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {flagType.FullName} is not a flag enum. Use the enum collection.");

        if (Flags.TryGetValue(flagType, out var value))
            return (T)value;

        throw new KeyNotFoundException($"Enum of type {flagType.FullName} was not found in the collection");
    }

    /// <summary>
    ///     Determines if the flag collection contains the specified flag
    /// </summary>
    /// <param name="flag">The flag value to check for</param>
    /// <typeparam name="T">The type of the flag</typeparam>
    /// <returns>
    ///     <c>true</c> if a flag of the given value was found, and that flag contains the value specified, otherwise
    ///     <c>false</c>
    /// </returns>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    public bool HasFlag<T>(T flag) where T: Enum
    {
        var flagType = typeof(T);

        return HasFlag(flagType, flag);
    }

    /// <summary>
    ///     Determines if the flag collection contains the specified flag
    /// </summary>
    /// <param name="type">the type of the flag</param>
    /// <param name="value">The flag value to check for</param>
    /// <returns>
    ///     <c>true</c> if a flag of the given value was found, and that flag contains the value specified, otherwise
    ///     <c>false</c>
    /// </returns>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    public bool HasFlag(Type type, Enum value)
    {
        if (!type.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {type.FullName} is not a flag enum. Use the enum collection.");

        if (Flags.TryGetValue(type, out var existingValue))
        {
            var flagValue = Convert.ToUInt64(existingValue);
            var valueToCheck = Convert.ToUInt64(value);

            return (flagValue & valueToCheck) == valueToCheck;
        }

        return false;
    }

    /// <summary>
    ///     Attempts to remove the value of the flag of the specified type from the collection
    /// </summary>
    /// <param name="flag">The value of the flag to remove</param>
    /// <typeparam name="T">The type of the flag</typeparam>
    public void RemoveFlag<T>(T flag) where T: Enum => RemoveFlag(typeof(T), flag);

    /// <summary>
    ///     Attempts to remove the value of the flag of the specified type from the collection
    /// </summary>
    /// <param name="flagType">The type of the flag</param>
    /// <param name="flagValue">The value of the flag to remove</param>
    public void RemoveFlag(Type flagType, Enum flagValue)
    {
        if (Flags.TryGetValue(flagType, out var value))
            Flags[flagType] = (Enum)Enum.ToObject(flagType, Convert.ToUInt64(value) & ~Convert.ToUInt64(flagValue));
    }

    /// <summary>
    ///     Attempts to retreive the value of the flag of the specified type
    /// </summary>
    /// <param name="value">The value of the flag retreived</param>
    /// <typeparam name="T">The type of the flag</typeparam>
    /// <returns><c>true</c> if a flag with the specified type was found, otherwise <c>false</c></returns>
    public bool TryGetFlag<T>([MaybeNullWhen(false)] out T value) where T: Enum
    {
        value = default;
        var flagType = typeof(T);

        if (!TryGetFlag(flagType, out var enumValue))
            return false;

        value = (T)enumValue;

        return true;
    }

    /// <summary>
    ///     Attempts to retreive the value of the flag of the specified type
    /// </summary>
    /// <param name="flagType">The type of the flag</param>
    /// <param name="enumValue">The value of the flag retreived</param>
    /// <returns><c>true</c> if a flag with the specified type was found, otherwise <c>false</c></returns>
    /// <exception cref="InvalidOperationException">Enum must have flag attribute</exception>
    public bool TryGetFlag(Type flagType, [MaybeNullWhen(false)] out Enum enumValue)
    {
        if (!flagType.IsFlagEnum())
            throw new InvalidOperationException($"Enum of type {flagType.FullName} is not a flag enum. Use the enum collection.");

        return Flags.TryGetValue(flagType, out enumValue);
    }
}