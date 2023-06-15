using System.Text.RegularExpressions;

namespace Chaos.Geometry.Definitions;

internal static partial class RegexCache
{
    internal static readonly Regex LOCATION_REGEX = GenerateLocationRegex();
    internal static readonly Regex POINT_REGEX = GeneratePointRegex();

    [GeneratedRegex(@"(.+?)(?::| |: )\(?(\d+)(?:,| |, )(\d+)\)?", RegexOptions.Compiled)]
    private static partial Regex GenerateLocationRegex();

    [GeneratedRegex(@"\(?(\d+)(?:,| |, )(\d+)\)?", RegexOptions.Compiled)]
    private static partial Regex GeneratePointRegex();
}