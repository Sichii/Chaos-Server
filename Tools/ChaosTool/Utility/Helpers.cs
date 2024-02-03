using System.IO;
using System.Text;
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
}