using System.Globalization;
using System.Windows.Data;
using Chaos.Extensions.Common;

namespace Chaos.Wpf.Converters;

/// <summary>
///     Converts primitive types (including their nullable counterparts) to strings
/// </summary>
public sealed class NullablePrimitiveToStringConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value?.ToString();

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
            return null;

        var underlyingType = Nullable.GetUnderlyingType(targetType);

        if (underlyingType is not null)
            targetType = underlyingType;

        //parse enums
        if (targetType.IsEnum)
        {
            if (Enum.TryParse(targetType, value.ToString()!, out var result))
                return result;

            return null;
        }

        //try to parse IParsable<T> types
        if (targetType.HasInterface(typeof(IParsable<>)))
        {
            var tryParse = targetType.GetMethod(
                nameof(IParsable<int>.TryParse),
                [
                    typeof(string),
                    typeof(IFormatProvider),
                    targetType.MakeByRefType()
                ]);

            if (tryParse is null)
                return null;

            var parameters = new[]
            {
                value,
                null,
                null
            };
            var success = (bool)tryParse.Invoke(null, parameters)!;

            if (success)
                return parameters[2];

            return null;
        }

        return null;
    }
}