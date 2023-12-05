using System.Text;
using Namotion.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Chaos.Schemas.Extensions;

public static class TypeExtensions
{
    public static string GetXmlDocs(this Type type, string? propertyName = null)
    {
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
}