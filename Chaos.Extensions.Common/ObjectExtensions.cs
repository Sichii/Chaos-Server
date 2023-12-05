using System.ComponentModel;

namespace Chaos.Extensions.Common;

/// <summary>
///     Contains extension methods for any object
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     Gets the value of the Description attribute for the given member
    /// </summary>
    /// <returns></returns>
    public static string GetDescription(this Type type, object memberName)
    {
        var member = type.GetMember(memberName.ToString()!);

        if (member.Length > 0)
        {
            var attributes = member[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return ((DescriptionAttribute)attributes[0]).Description;
        }

        return string.Empty;
    }
}