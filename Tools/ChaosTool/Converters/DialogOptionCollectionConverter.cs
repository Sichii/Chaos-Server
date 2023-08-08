using System.Globalization;
using System.Windows.Data;
using Chaos.Schemas.Data;
using ChaosTool.Extensions;

namespace ChaosTool.Converters;

internal sealed class DialogOptionCollectionConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new DialogOptionCollectionConverter();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumerable<DialogOptionSchema> dialogOptionSchemas)
            return string.Empty;

        return dialogOptionSchemas.To2LinesPerItem(schema => schema.OptionText, schema => schema.DialogKey);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotImplementedException();
}