using System.Collections;
using System.IO;
using System.Text;
using Chaos.Common.Converters;
using Chaos.Extensions.Common;
using ChaosTool.ViewModel.Abstractions;
using Namotion.Reflection;

namespace ChaosTool.Utility;

public static class Helpers
{
    public static string GetPropertyDocs<T>(string? propertyName = null)
    {
        var type = typeof(T);

        var opts = new XmlDocsOptions
        {
            FormattingMode = XmlDocsFormattingMode.None,
            ResolveExternalXmlDocs = false
        };
        var builder = new StringBuilder();

        if (string.IsNullOrEmpty(propertyName))
        {
            builder.AppendLine(type.GetXmlDocsSummary(opts));
            builder.AppendLine(type.GetXmlDocsRemarks(opts));
        } else
        {
            var prop = type.GetProperty(propertyName);

            builder.AppendLine(prop!.GetXmlDocsSummary(opts));
            builder.AppendLine(prop!.GetXmlDocsRemarks(opts));
        }

        return builder.ToString()
                      .Trim();
    }

    public static bool IsDefault<T>(this T? value) where T: new() => value!.Equals(new T());

    public static bool ValidatePreSave(ViewModelBase viewModel, string templateKey)
    {
        var fileName = Path.GetFileNameWithoutExtension(viewModel.Path);

        return fileName.EqualsI(templateKey);
    }

    #region Primitive Manipulation
    public static IEnumerable<string> GetEnumNames<T>()
    {
        var type = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType is not null)
            type = underlyingType;

        if (!type.IsEnum)
            throw new InvalidOperationException($"{type.Name} is not an enum");

        if (underlyingType is not null)
            return Enum.GetNames(type)
                       .Prepend(string.Empty);

        return Enum.GetNames(type);
    }

    public static T? ParsePrimitive<T>(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return default;

        return PrimitiveConverter.Convert<T>(str);
    }

    public static string SelectPrimitive<T>(T value, IEnumerable enumerable)
    {
        var strings = enumerable.OfType<string>();

        if (value is null)
            return strings.First(string.IsNullOrEmpty);

        var valueStr = value.ToString()!;

        return strings.First(str => str.EqualsI(valueStr));
    }
    #endregion
}