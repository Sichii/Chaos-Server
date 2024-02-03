namespace Chaos.Common.Converters;

/// <summary>
///     Provides methods to convert values between different primitive types.
/// </summary>
public static class PrimitiveConverter
{
    /// <summary>
    ///     Converts the given value to the specified type.
    /// </summary>
    /// <typeparam name="T">
    ///     The target type to convert the value to.
    /// </typeparam>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     The converted value of the specified type.
    /// </returns>
    public static T Convert<T>(object value) => (T)Convert(typeof(T), value);

    /// <summary>
    ///     Parses the given string to the specified type.
    /// </summary>
    /// <param name="str">
    ///     The string to parse the value from
    /// </param>
    /// <param name="formatProvider">
    ///     The format provider to use when parsing the type
    /// </param>
    /// <typeparam name="T">
    ///     The type to parse
    /// </typeparam>
    /// <returns>
    ///     The parse value
    /// </returns>
    public static T Convert<T>(string str, IFormatProvider? formatProvider = null) where T: IParsable<T> => T.Parse(str, formatProvider);

    /// <summary>
    ///     Converts the given value to the specified type.
    /// </summary>
    /// <param name="type">
    ///     The target Type to convert the value to.
    /// </param>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     The converted value of the specified type.
    /// </returns>
    public static object Convert(Type type, object value)
    {
        if (value is string && (type == typeof(string)))
            return value;

        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        if (nullableUnderlyingType != null)
            type = nullableUnderlyingType;

        if (type.IsEnum)
            return Enum.Parse(type, value.ToString()!, true);

        //if it's a primitive, convert it
        return System.Convert.ChangeType(value, type);
    }

    /// <summary>
    ///     Attempts to parses the given string to the specified type.
    /// </summary>
    /// <param name="str">
    ///     The string to parse the value from
    /// </param>
    /// <param name="formatProvider">
    ///     The format provider to use when parsing the type
    /// </param>
    /// <typeparam name="T">
    ///     The type to parse
    /// </typeparam>
    public static T? TryConvert<T>(string str, IFormatProvider? formatProvider = null) where T: IParsable<T>
    {
        if (string.IsNullOrEmpty(str))
            return default;

        return Convert<T>(str, formatProvider);
    }

    /// <summary>
    ///     Attempts to convert the given value to the specified type.
    /// </summary>
    /// <typeparam name="T">
    ///     The target type to convert the value to.
    /// </typeparam>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    public static T? TryConvert<T>(object? value)
    {
        if (value is null)
            return default;

        return Convert<T>(value);
    }
}