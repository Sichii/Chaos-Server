namespace Chaos.Common.Comparers;

/// <summary>
///     A natural string comparer intended to work similarly to how windows orders files. Strings with numbers will be
///     compared numerically, rather than lexicographically.
/// </summary>
public sealed class NaturalStringComparer : IComparer<string>
{
    /// <summary>
    ///     Comparers are basically static methods, no point in creating many instances
    /// </summary>
    public static NaturalStringComparer Instance { get; } = new();

    /// <inheritdoc />
    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (x == null)
            return -1;

        if (y == null)
            return 1;

        var ix = 0;
        var iy = 0;

        while ((ix < x.Length) && (iy < y.Length))
            if (char.IsDigit(x[ix]) && char.IsDigit(y[iy]))
            {
                // Compare numeric portions
                var numResult = CompareNumericPortion(
                    x,
                    y,
                    ref ix,
                    ref iy);

                if (numResult != 0)
                    return numResult;
            } else
            {
                // Compare single characters
                if (x[ix] != y[iy])
                    return x[ix]
                        .CompareTo(y[iy]);

                ix++;
                iy++;
            }

        // Handle case where one string is exhausted
        if (ix < x.Length)
            return 1; // x has remaining characters

        if (iy < y.Length)
            return -1; // y has remaining characters

        return 0; // both strings exhausted simultaneously
    }

    private static int CompareNumericPortion(
        string x,
        string y,
        ref int ix,
        ref int iy)
    {
        // Skip leading zeros in x
        while ((ix < x.Length) && (x[ix] == '0'))
            ix++;

        // Skip leading zeros in y  
        while ((iy < y.Length) && (y[iy] == '0'))
            iy++;

        // Count significant digits
        var xDigitStart = ix;
        var yDigitStart = iy;

        while ((ix < x.Length) && char.IsDigit(x[ix]))
            ix++;

        while ((iy < y.Length) && char.IsDigit(y[iy]))
            iy++;

        var xDigitCount = ix - xDigitStart;
        var yDigitCount = iy - yDigitStart;

        // Compare by digit count first (longer number is larger)
        if (xDigitCount != yDigitCount)
            return xDigitCount.CompareTo(yDigitCount);

        // Same number of significant digits, compare lexicographically
        for (var i = 0; i < xDigitCount; i++)
        {
            var xDigit = x[xDigitStart + i];
            var yDigit = y[yDigitStart + i];

            if (xDigit != yDigit)
                return xDigit.CompareTo(yDigit);
        }

        return 0; // Numbers are equal
    }
}