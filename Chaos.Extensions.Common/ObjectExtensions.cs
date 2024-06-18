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
    /// <returns>
    /// </returns>
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

    /// <summary>
    ///     Executes the specified <paramref name="func" /> and catches any exceptions that occur. (Does not rethrow, should
    ///     only be used as a shorthand for unimportant things)
    /// </summary>
    public static TReturn? TryCatch<T, TReturn>(this T obj, Func<T, TReturn?> func, Func<Exception, TReturn?> exceptionHandler)
    {
        try
        {
            return func(obj);
        } catch (Exception ex)
        {
            return exceptionHandler(ex);
        }
    }

    /// <summary>
    ///     Executes the specified <paramref name="action" /> and catches any exceptions that occur. (Does not rethrow, should
    ///     only be used as a shorthand for unimportant things)
    /// </summary>
    public static void TryCatch<T>(this T obj, Action action, Action<Exception> exceptionHandler)
    {
        try
        {
            action();
        } catch (Exception ex)
        {
            exceptionHandler(ex);
        }
    }
}