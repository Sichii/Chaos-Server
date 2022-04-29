namespace Chaos.Core.Extensions;

public static class StringExtensions
{
    public static bool ContainsI(this string str1, string str2) => str1.Contains(str2, StringComparison.OrdinalIgnoreCase);
    public static bool EqualsI(this string str1, string str2) => str1.Equals(str2, StringComparison.OrdinalIgnoreCase);

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