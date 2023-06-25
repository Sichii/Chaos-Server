using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosTool.Converters;

public class MarginConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => new Thickness(
        0,
        System.Convert.ToDouble(value),
        0,
        0);

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) => null!;
}