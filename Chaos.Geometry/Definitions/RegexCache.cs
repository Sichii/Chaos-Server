#region
using System.Text.RegularExpressions;
#endregion

namespace Chaos.Geometry.Definitions;

// ReSharper disable once PartialTypeWithSinglePart
internal static partial class RegexCache
{
    [GeneratedRegex(@"(.+?)(?::| |: )\(?(\d+)(?:,| |, )(\d+)\)?", RegexOptions.Compiled)]
    internal static partial Regex LocationRegex { get; }

    [GeneratedRegex(@"\(?(\d+)(?:,| |, )(\d+)\)?", RegexOptions.Compiled)]
    internal static partial Regex PointRegex { get; }
}