namespace Chaos.Extensions.Common;

public static class StringExtensions
{
    /// <summary>
    ///     Determines if a string contains another string in a case insensitive manner.
    /// </summary>
    public static bool ContainsI(this string str1, string str2) => str1.Contains(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines if two strings are equivalent in a case insensitive manner.
    /// </summary>
    public static bool EqualsI(this string str1, string str2) => str1.Equals(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Capitolizes the first letter in a string
    /// </summary>
    /// <exception cref="ArgumentNullException">input is null</exception>
    /// <exception cref="ArgumentException">input is empty</exception>
    public static string FirstUpper(this string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        return input switch
        {
            "" => throw new ArgumentException($@"{nameof(input)} cannot be empty", nameof(input)),
            _  => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }
}