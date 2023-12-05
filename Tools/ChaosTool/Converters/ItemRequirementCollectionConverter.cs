using System.Globalization;
using System.Windows.Data;
using Chaos.Schemas.Data;
using ChaosTool.Extensions;

namespace ChaosTool.Converters;

public sealed class ItemRequirementCollectionConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new ItemRequirementCollectionConverter();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not IEnumerable<ItemRequirementSchema> itemRequirementSchemas)
            return string.Empty;

        return itemRequirementSchemas.To2LinesPerItem(schema => schema.ItemTemplateKey, schema => schema.AmountRequired.ToString());
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotImplementedException();
}