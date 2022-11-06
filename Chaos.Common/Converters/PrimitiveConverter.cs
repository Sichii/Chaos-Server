namespace Chaos.Common.Converters;

public static class PrimitiveConverter
{
    public static T Convert<T>(object value) => (T)Convert(typeof(T), value);

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