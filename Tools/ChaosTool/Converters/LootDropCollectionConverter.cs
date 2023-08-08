using System.Globalization;
using System.Windows.Data;
using Chaos.Schemas.Data;
using ChaosTool.Extensions;

namespace ChaosTool.Converters;

public sealed class LootDropCollectionConverter : IValueConverter
{
    public static readonly IValueConverter Instance = new LootDropCollectionConverter();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumerable<LootDropSchema> dialogOptionSchemas)
            return string.Empty;

        return dialogOptionSchemas.To2LinesPerItem(
            schema => schema.ItemTemplateKey,
            schema => schema.DropChance.ToString(CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        throw new NotImplementedException();
}