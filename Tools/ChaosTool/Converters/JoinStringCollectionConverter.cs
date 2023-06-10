using System.Globalization;
using System.Windows.Data;
using ChaosTool.Extensions;

namespace ChaosTool.Converters;

internal sealed class JoinStringCollectionConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new JoinStringCollectionConverter();

    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumerable<string> list)
            return string.Empty;

        return list.ToLinePerString();
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => throw new NotImplementedException();
}