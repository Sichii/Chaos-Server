using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.CodeAnalysis.CodeActions;
using RoslynPad.Roslyn.CodeActions;

namespace ChaosTool.Converters;

internal sealed class CodeActionsConverter : MarkupExtension, IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => ((CodeAction)value!).GetCodeActions();

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}