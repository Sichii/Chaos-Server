using System.Text.RegularExpressions;

namespace Chaos.Geometry.Definitions;

public static class RegexCache
{
    public static readonly Regex LOCATION_REGEX = new(@"(.+)(?::| )\(?(\d+),? ?(\d+)\)?", RegexOptions.Compiled);
    public static readonly Regex POINT_REGEX = new(@"\(?(\d+),? ?(\d+)\)?", RegexOptions.Compiled);
}