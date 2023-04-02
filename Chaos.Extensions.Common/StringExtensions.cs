using JetBrains.Annotations;

namespace Chaos.Extensions.Common;

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
    ///     <c>true</c> if the value parameter occurs within this string, or if value is the empty string (""); otherwise, <c>false</c>
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
            "" => throw new ArgumentException($@"{nameof(input)} cannot be empty", nameof(input)),
            _  => string.Concat(new ReadOnlySpan<char>(char.ToUpper(input[0])), input.AsSpan(1))
        };
    }

    /// <summary>
    ///     Injects the parameters into the string, replacing the placeholders with the parameters
    /// </summary>
    /// <param name="str1">The string in which to replace placeholders with parameters</param>
    /// <param name="parameters">The parameters to inject into the string</param>
    /// <returns>A string where the placeholders are replaced with the given parameters</returns>
    public static string Inject([StructuredMessageTemplate] this string str1, params object[] parameters)
    {
        var paramStrs = new string[parameters.Length];
        var paramsTotalLength = 0;

        //convert all parameters to strings and aggregate the length of the parameters
        for (var i = 0; i < parameters.Length; i++)
        {
            var paramStr = parameters[i].ToString() ?? string.Empty;
            paramsTotalLength += paramStr.Length;
            paramStrs[i] = paramStr;
        }

        //initialize indexes, allocate result span of max possible length
        var paramIndex = 0;
        var strIndex = 0;
        var resIndex = 0;
        var strSpan = str1.AsSpan();
        Span<char> result = stackalloc char[strSpan.Length + paramsTotalLength];
        var startPhIndex = -1;

        while (strIndex < strSpan.Length)
        {
            var nextIndex = strIndex + 1;

            switch (strSpan[strIndex])
            {
                //handle double curly braces
                case '{' when (nextIndex < strSpan.Length) && (strSpan[nextIndex] == '{'):
                    result[resIndex++] = '{';
                    strIndex += 2;

                    break;
                //handle start of placeholder
                case '{':
                    startPhIndex = strIndex;
                    strIndex += 1;

                    break;
                //handle double curly braces
                case '}' when (nextIndex < strSpan.Length) && (strSpan[nextIndex] == '}'):
                    result[resIndex++] = '}';
                    strIndex += 2;

                    break;
                //handle end of placeholder
                case '}' when startPhIndex >= 0:
                    if (paramIndex >= paramStrs.Length)
                        throw new ArgumentException(
                            "The number of parameters is less than the number of placeholders in the string",
                            nameof(parameters));

                    var parameterValue = paramStrs[paramIndex++];
                    parameterValue.AsSpan().CopyTo(result[resIndex..]);
                    resIndex += parameterValue.Length;
                    strIndex += 1;
                    startPhIndex = -1;

                    break;
                //handle normal character
                default:
                    if (startPhIndex < 0)
                        result[resIndex++] = strSpan[strIndex];

                    strIndex++;

                    break;
            }
        }

        return result[..resIndex].ToString();
    }

    /// <summary>
    ///     Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string
    ///     when compared case insensitively
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="oldValue">The string to be replaced</param>
    /// <param name="newValue">The string to replace all occurrences of oldValue</param>
    /// <returns>
    ///     A string that is equivalent to the current string except that all instances of <paramref name="oldValue" /> are replaced with
    ///     <paramref name="newValue" />. If <paramref name="oldValue" /> is not found in the current instance, the method returns the current
    ///     instance unchanged
    /// </returns>
    public static string ReplaceI(this string str1, string oldValue, string newValue) =>
        str1.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines whether the beginning of this string instance matches the specified string when compared case insensitively.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2">The string to compare</param>
    /// <returns><c>true</c> if this instance begins with value; otherwise, <c>false</c></returns>
    public static bool StartsWithI(this string str1, string str2) => str1.StartsWith(str2, StringComparison.OrdinalIgnoreCase);
}