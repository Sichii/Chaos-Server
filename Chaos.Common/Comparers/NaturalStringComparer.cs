namespace Chaos.Common.Comparers;

/// <summary>
///     A natural string comparer intended to work similarly to how windows orders files. Strings with numbers will be
///     compared numerically, rather than lexicographically.
/// </summary>
public class NaturalStringComparer : IComparer<string>
{
    /// <inheritdoc />
    public int Compare(string? x, string? y)
    {
        if ((x == null) && (y == null))
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
                var lx = 0;
                var ly = 0;

                while ((ix < x.Length) && char.IsDigit(x[ix]))
                {
                    lx = lx * 10 + (x[ix] - '0');
                    ix++;
                }

                while ((iy < y.Length) && char.IsDigit(y[iy]))
                {
                    ly = ly * 10 + (y[iy] - '0');
                    iy++;
                }

                if (lx != ly)
                    return lx.CompareTo(ly);
            } else
            {
                if (x[ix] != y[iy])
                    return x[ix]
                        .CompareTo(y[iy]);

                ix++;
                iy++;
            }

        return x.Length.CompareTo(y.Length);
    }
}