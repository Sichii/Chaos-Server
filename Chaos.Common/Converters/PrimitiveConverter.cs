namespace Chaos.Common.Converters;

/// <summary>
///     Provides methods to convert values between different primitive types.
/// </summary>
public static class PrimitiveConverter
{
    /// <summary>
    ///     Converts the given value to the specified type.
    /// </summary>
    /// <typeparam name="T">The target type to convert the value to.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value of the specified type.</returns>
    public static T Convert<T>(object value) => (T)Convert(typeof(T), value);

    /// <summary>
    ///     Converts the given value to the specified type.
    /// </summary>
    /// <param name="type">The target Type to convert the value to.</param>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted value of the specified type.</returns>
    public static object Convert(Type type, object value)
    {
        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        if (nullableUnderlyingType != null)
            type = nullableUnderlyingType;

        if (type.IsEnum)
            return Enum.Parse(type, value.ToString()!, true);

        //if it's a primitive, convert it
        return System.Convert.ChangeType(value, type);
    }
}