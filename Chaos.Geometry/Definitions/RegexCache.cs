using System.Text.RegularExpressions;

namespace Chaos.Geometry.Definitions;

public static partial class RegexCache
{
    public static readonly Regex LOCATION_REGEX = GeneratedLocationRegex();
    public static readonly Regex POINT_REGEX = GeneratedPointRegex();

    [RegexGenerator(@"(.+?)(?::| |: )\(?(\d+),? ?(\d+)\)?", RegexOptions.Compiled)]
    private static partial Regex GeneratedLocationRegex();

    [RegexGenerator(@"\(?(\d+),? ?(\d+)\)?", RegexOptions.Compiled)]
    private static partial Regex GeneratedPointRegex();
}