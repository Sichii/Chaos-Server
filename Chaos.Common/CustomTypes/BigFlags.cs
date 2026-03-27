#region
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Chaos.Common.Abstractions;
#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Common.CustomTypes;

/// <summary>
///     Utility class for working with BigFlagsValue types. Provides both generic (compile-time) and non-generic (runtime)
///     methods. Similar to the Enum class for working with enum types.
/// </summary>
public static class BigFlags
{
    /// <summary>
    ///     Creates a BigFlagsValue with the given BigInteger value.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="value">
    ///     The BigInteger value
    /// </param>
    /// <returns>
    ///     A BigFlagsValue
    /// </returns>
    public static BigFlagsValue<TMarker> Create<TMarker>(BigInteger value) where TMarker: class => new(value);

    /// <summary>
    ///     Creates a BigFlagsValue with the given bit index.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="bitIndex">
    ///     The bit index to set
    /// </param>
    /// <returns>
    ///     A BigFlagsValue
    /// </returns>
    public static BigFlagsValue<TMarker> Create<TMarker>(int bitIndex) where TMarker: class => new(bitIndex);

    /// <summary>
    ///     Creates a BigFlagsValue for the specified marker type with the given BigInteger value.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="value">
    ///     The BigInteger value
    /// </param>
    /// <returns>
    ///     A BigFlagsValue as IBigFlagsValue
    /// </returns>
    public static IBigFlagsValue Create(Type markerType, BigInteger value)
    {
        ValidateMarkerType(markerType);

        var bigFlagsValueType = typeof(BigFlagsValue<>).MakeGenericType(markerType);

        return (IBigFlagsValue)Activator.CreateInstance(bigFlagsValueType, value)!;
    }

    /// <summary>
    ///     Creates a BigFlagsValue for the specified marker type with the given bit index.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="bitIndex">
    ///     The bit index to set
    /// </param>
    /// <returns>
    ///     A BigFlagsValue as IBigFlagsValue
    /// </returns>
    public static IBigFlagsValue Create(Type markerType, int bitIndex)
    {
        ValidateMarkerType(markerType);

        var bigFlagsValueType = typeof(BigFlagsValue<>).MakeGenericType(markerType);

        return (IBigFlagsValue)Activator.CreateInstance(bigFlagsValueType, bitIndex)!;
    }

    /// <summary>
    ///     Gets the name of a flag value.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="value">
    ///     The flag value to get the name for
    /// </param>
    /// <returns>
    ///     The name of the flag, or null if not found
    /// </returns>
    public static string? GetName<TMarker>(BigFlagsValue<TMarker> value) where TMarker: class => BigFlags<TMarker>.GetName(value);

    /// <summary>
    ///     Gets the name of a flag value for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="value">
    ///     The flag value to get the name for
    /// </param>
    /// <returns>
    ///     The name of the flag, or null if not found
    /// </returns>
    public static string? GetName(Type markerType, IBigFlagsValue value)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var getNameMethod = bigFlagsType.GetMethod(nameof(BigFlags<object>.GetName))
                            ?? throw new InvalidOperationException($"GetName method not found on {bigFlagsType.Name}");

        var result = getNameMethod.Invoke(null, [value]);

        return (string?)result;
    }

    /// <summary>
    ///     Returns the names of all defined flags.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <returns>
    ///     An enumerable of flag names
    /// </returns>
    public static IEnumerable<string> GetNames<TMarker>() where TMarker: class => BigFlags<TMarker>.GetNames();

    /// <summary>
    ///     Returns the names of all defined flags for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <returns>
    ///     An enumerable of flag names
    /// </returns>
    public static IEnumerable<string> GetNames(Type markerType)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var getNamesMethod = bigFlagsType.GetMethod(nameof(BigFlags<object>.GetNames))
                             ?? throw new InvalidOperationException($"GetNames method not found on {bigFlagsType.Name}");

        var result = getNamesMethod.Invoke(null, null);

        return (IEnumerable<string>)result!;
    }

    /// <summary>
    ///     Gets the None value.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <returns>
    ///     The None BigFlagsValue
    /// </returns>
    public static BigFlagsValue<TMarker> GetNone<TMarker>() where TMarker: class => BigFlags<TMarker>.None;

    /// <summary>
    ///     Gets the None value for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <returns>
    ///     The None BigFlagsValue as IBigFlagsValue
    /// </returns>
    public static IBigFlagsValue GetNone(Type markerType)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var noneField = bigFlagsType.GetField(nameof(BigFlags<object>.None))
                        ?? throw new InvalidOperationException($"None field not found on {bigFlagsType.Name}");

        return (IBigFlagsValue)noneField.GetValue(null)!;
    }

    /// <summary>
    ///     Returns all defined flag values.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <returns>
    ///     An enumerable of flag values
    /// </returns>
    public static IEnumerable<BigFlagsValue<TMarker>> GetValues<TMarker>() where TMarker: class => BigFlags<TMarker>.GetValues();

    /// <summary>
    ///     Returns all defined flag values for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <returns>
    ///     An enumerable of flag values as IBigFlagsValue
    /// </returns>
    public static IEnumerable<IBigFlagsValue> GetValues(Type markerType)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var getValuesMethod = bigFlagsType.GetMethod(nameof(BigFlags<object>.GetValues))
                              ?? throw new InvalidOperationException($"GetValues method not found on {bigFlagsType.Name}");

        var result = getValuesMethod.Invoke(null, null);

        return ((IEnumerable)result!).Cast<IBigFlagsValue>();
    }

    /// <summary>
    ///     Checks if a flag name is defined.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="name">
    ///     The name to check
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     True if the name is defined, false otherwise
    /// </returns>
    public static bool IsDefined<TMarker>(string name, bool ignoreCase = false) where TMarker: class
        => BigFlags<TMarker>.IsDefined(name, ignoreCase);

    /// <summary>
    ///     Checks if a flag value is defined.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="value">
    ///     The value to check
    /// </param>
    /// <returns>
    ///     True if the value is defined, false otherwise
    /// </returns>
    public static bool IsDefined<TMarker>(BigFlagsValue<TMarker> value) where TMarker: class => BigFlags<TMarker>.IsDefined(value);

    /// <summary>
    ///     Checks if a flag name is defined for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="name">
    ///     The name to check
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     True if the name is defined, false otherwise
    /// </returns>
    public static bool IsDefined(Type markerType, string name, bool ignoreCase = false)
    {
        if (!IsValidMarkerType(markerType))
            return false;

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var isDefinedMethod = bigFlagsType.GetMethod(
            nameof(BigFlags<object>.IsDefined),
            [
                typeof(string),
                typeof(bool)
            ]);

        if (isDefinedMethod == null)
            return false;

        var result = isDefinedMethod.Invoke(
            null,
            [
                name,
                ignoreCase
            ]);

        return (bool)result!;
    }

    /// <summary>
    ///     Checks if a flag value is defined for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="value">
    ///     The value to check
    /// </param>
    /// <returns>
    ///     True if the value is defined, false otherwise
    /// </returns>
    public static bool IsDefined(Type markerType, IBigFlagsValue value)
    {
        if (!IsValidMarkerType(markerType))
            return false;

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var isDefinedMethod = bigFlagsType.GetMethod(
            nameof(BigFlags<object>.IsDefined),
            [typeof(BigFlagsValue<>).MakeGenericType(markerType)]);

        if (isDefinedMethod == null)
            return false;

        var result = isDefinedMethod.Invoke(null, [value]);

        return (bool)result!;
    }

    private static bool IsValidMarkerType(Type markerType)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (markerType == null)
            return false;

        if (!markerType.IsClass)
            return false;

        // Check if it's used as a marker for BigFlags
        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        return markerType.IsAssignableTo(bigFlagsType);
    }

    // ============================================================
    // Generic versions (type-safe, compile-time)
    // ============================================================

    /// <summary>
    ///     Parses a flag name into its corresponding BigFlagsValue.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     The parsed BigFlagsValue
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the name is not a defined flag
    /// </exception>
    public static BigFlagsValue<TMarker> Parse<TMarker>(string name, bool ignoreCase = false) where TMarker: class
        => BigFlags<TMarker>.Parse(name, ignoreCase);

    // ============================================================
    // Non-generic versions (runtime type)
    // ============================================================

    /// <summary>
    ///     Parses a flag name into its corresponding BigFlagsValue for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <returns>
    ///     The parsed BigFlagsValue as IBigFlagsValue
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the name is not a defined flag or markerType is invalid
    /// </exception>
    public static IBigFlagsValue Parse(Type markerType, string name, bool ignoreCase = false)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var parseMethod = bigFlagsType.GetMethod(
                              nameof(BigFlags<object>.Parse),
                              [
                                  typeof(string),
                                  typeof(bool)
                              ])
                          ?? throw new InvalidOperationException($"Parse method not found on {bigFlagsType.Name}");

        try
        {
            var result = parseMethod.Invoke(
                null,
                [
                    name,
                    ignoreCase
                ]);

            return (IBigFlagsValue)result!;
        } catch (Exception ex) when (ex.InnerException is ArgumentException)
        {
            throw ex.InnerException;
        }
    }

    /// <summary>
    ///     Returns a string representation of the flag value showing flag names.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="value">
    ///     The flag value to convert to string
    /// </param>
    /// <returns>
    ///     A string representation showing flag names
    /// </returns>
    public static string ToString<TMarker>(BigFlagsValue<TMarker> value) where TMarker: class => BigFlags<TMarker>.ToString(value);

    /// <summary>
    ///     Returns a string representation of the flag value for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="value">
    ///     The flag value to convert to string
    /// </param>
    /// <returns>
    ///     A string representation showing flag names
    /// </returns>
    public static string ToString(Type markerType, IBigFlagsValue value)
    {
        ValidateMarkerType(markerType);

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);

        var toStringMethod
            = bigFlagsType.GetMethod(nameof(BigFlags<object>.ToString), [typeof(BigFlagsValue<>).MakeGenericType(markerType)])
              ?? throw new InvalidOperationException($"ToString method not found on {bigFlagsType.Name}");

        var result = toStringMethod.Invoke(null, [value]);

        return (string)result!;
    }

    /// <summary>
    ///     Tries to parse a flag name into its corresponding BigFlagsValue.
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <param name="value">
    ///     The parsed flag value
    /// </param>
    /// <returns>
    ///     True if parsing succeeded, false otherwise
    /// </returns>
    public static bool TryParse<TMarker>(string name, bool ignoreCase, out BigFlagsValue<TMarker> value) where TMarker: class
        => BigFlags<TMarker>.TryParse(name, ignoreCase, out value);

    /// <summary>
    ///     Tries to parse a flag name into its corresponding BigFlagsValue (case-sensitive).
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type
    /// </typeparam>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="value">
    ///     The parsed flag value
    /// </param>
    /// <returns>
    ///     True if parsing succeeded, false otherwise
    /// </returns>
    public static bool TryParse<TMarker>(string name, out BigFlagsValue<TMarker> value) where TMarker: class
        => BigFlags<TMarker>.TryParse(name, out value);

    /// <summary>
    ///     Tries to parse a flag name into its corresponding BigFlagsValue for the specified marker type.
    /// </summary>
    /// <param name="markerType">
    ///     The marker type (TMarker)
    /// </param>
    /// <param name="name">
    ///     The name of the flag to parse
    /// </param>
    /// <param name="ignoreCase">
    ///     Whether to ignore case when matching names
    /// </param>
    /// <param name="value">
    ///     The parsed flag value, or null if parsing failed
    /// </param>
    /// <returns>
    ///     True if parsing succeeded, false otherwise
    /// </returns>
    public static bool TryParse(
        Type markerType,
        string name,
        bool ignoreCase,
        [MaybeNullWhen(false)] out IBigFlagsValue value)
    {
        value = null;

        if (!IsValidMarkerType(markerType))
            return false;

        var bigFlagsType = typeof(BigFlags<>).MakeGenericType(markerType);
        var bigFlagsValueType = typeof(BigFlagsValue<>).MakeGenericType(markerType);

        var tryParseMethod = bigFlagsType.GetMethod(
            nameof(BigFlags<object>.TryParse),
            [
                typeof(string),
                typeof(bool),
                bigFlagsValueType.MakeByRefType()
            ]);

        if (tryParseMethod == null)
            return false;

        var parameters = new object?[]
        {
            name,
            ignoreCase,
            null
        };
        var success = (bool)tryParseMethod.Invoke(null, parameters)!;

        if (success)
            value = (IBigFlagsValue)parameters[2]!;

        return success;
    }

    /// <summary>
    ///     Tries to parse a flag name into its corresponding BigFlagsValue (case-sensitive).
    /// </summary>
    public static bool TryParse(Type markerType, string name, [MaybeNullWhen(false)] out IBigFlagsValue value)
        => TryParse(
            markerType,
            name,
            false,
            out value);

    private static void ValidateMarkerType(Type markerType)
    {
        if (!IsValidMarkerType(markerType))
            throw new ArgumentException(
                $"Type {markerType.FullName} is not a valid marker type. It must be a class that inherits from BigFlags<{markerType.Name}>",
                nameof(markerType));
    }
}