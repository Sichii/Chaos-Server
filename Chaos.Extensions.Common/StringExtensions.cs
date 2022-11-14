namespace Chaos.Extensions.Common;

public static class StringExtensions
{
    public static string CenterAlign(this string str1, int width)
    {
        if (str1.Length > width)
            return str1[..width];

        var leftPadding = (width - str1.Length) / 2;
        var rightPadding = width - str1.Length - leftPadding;

        return new string(' ', leftPadding) + str1 + new string(' ', rightPadding);
    }

    /// <summary>
    ///     Determines if a string contains another string in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this string str1, string str2) => str1.Contains(str2, StringComparison.OrdinalIgnoreCase);

    public static bool EndsWithI(this string str1, string str2) => str1.EndsWith(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines if two strings are equivalent in a case insensitive manner.
    /// </summary>
    public static bool EqualsI(this string str1, string str2) => str1.Equals(str2, StringComparison.OrdinalIgnoreCase);

    public static Type FindType(this string typeName, string baseType)
    {
        var possibleTypes = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .Where(a => !a.IsDynamic)
                                     .SelectMany(a => a.GetTypes())
                                     .Where(asmType => !asmType.IsInterface && !asmType.IsAbstract)
                                     .Where(asmType => asmType.Name.EqualsI(typeName));

        return possibleTypes.Single(type => type.BaseType!.Name.EqualsI(baseType));
    }

    /// <summary>
    ///     Capitolizes the first letter in a string
    /// </summary>
    /// <exception cref="ArgumentNullException">input is null</exception>
    /// <exception cref="ArgumentException">input is empty</exception>
    public static string FirstUpper(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input switch
        {
            "" => throw new ArgumentException($@"{nameof(input)} cannot be empty", nameof(input)),
            _  => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }

    public static string ReplaceI(this string str1, string old, string @new) => str1.Replace(old, @new, StringComparison.OrdinalIgnoreCase);

    public static bool StartsWithI(this string str1, string str2) => str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);

    public static bool StartWithI(this string str1, string str2) => str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
}