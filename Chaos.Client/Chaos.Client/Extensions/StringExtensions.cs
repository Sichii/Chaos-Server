namespace Chaos.Client.Extensions;

/// <summary>
///     Provides extensions methods for <see cref="System.String" />
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Center aligns a string in a field of a specified width.
    /// </summary>
    /// <param name="str1">The string to center-align</param>
    /// <param name="width">The width of the field</param>
    public static string CenterAlign(this string str1, int width)
    {
        if (str1.Length > width)
            return str1[..width];

        var leftPadding = (width - str1.Length) / 2;
        var rightPadding = width - str1.Length - leftPadding;

        return new string(' ', leftPadding) + str1 + new string(' ', rightPadding);
    }

    /// <summary>
    ///     Returns a value indicating whether a specified string occurs within this string when compared case insensitively.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2">The string to seek</param>
    /// <returns>
    ///     <c>true</c> if the value parameter occurs within this string, or if value is the empty string (""); otherwise,
    ///     <c>false</c>
    /// </returns>
    public static bool ContainsI(this string str1, string str2) => str1.Contains(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines whether the end of this string instance matches the specified string when compared case insensitively.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2">The string to compare to the substring at the end of this instance</param>
    /// <returns><c>true</c> if the value parameter matches the end of this string; otherwise, <c>false</c></returns>
    public static bool EndsWithI(this string str1, string str2) => str1.EndsWith(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines whether this string and a specified String object have the same value when compared case insensitively.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2">The string to compare to this instance</param>
    /// <returns><c>true</c> if the value of the value parameter is the same as this string; otherwise, <c>false</c></returns>
    public static bool EqualsI(this string str1, string str2) => str1.Equals(str2, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Capitalizes the first letter in a string
    /// </summary>
    /// <exception cref="ArgumentNullException">input is null</exception>
    /// <exception cref="ArgumentException">input is empty</exception>
    public static string FirstUpper(this string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input switch
        {
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _  => string.Concat(char.ToUpper(input[0]), input[1..])
        };
    }

    /// <summary>
    ///     Returns a new string in which all occurrences of a specified string in the current instance are replaced with
    ///     another specified string
    ///     when compared case insensitively
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="oldValue">The string to be replaced</param>
    /// <param name="newValue">The string to replace all occurrences of oldValue</param>
    /// <returns>
    ///     A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are
    ///     replaced with
    ///     <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method
    ///     returns the current
    ///     instance unchanged
    /// </returns>
    public static string ReplaceI(this string str1, string oldValue, string newValue)
        => str1.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines whether the beginning of this string instance matches the specified string when compared case
    ///     insensitively.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2">The string to compare</param>
    /// <returns><c>true</c> if this instance begins with value; otherwise, <c>false</c></returns>
    public static bool StartsWithI(this string str1, string str2) => str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
}