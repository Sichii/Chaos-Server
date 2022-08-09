using System.Text;

namespace Chaos.Core.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    ///     Converts an object to a string, printing property name - value pairs
    /// </summary>
    public static string ToStringSynthetic(this object obj)
    {
        var builder = new StringBuilder();
        var type = obj.GetType();
        builder.AppendLine();

        foreach (var property in type.GetProperties())
            builder.AppendLine($"{property.Name}: {property.GetValue(obj)}");

        return builder.ToString();
    }
}