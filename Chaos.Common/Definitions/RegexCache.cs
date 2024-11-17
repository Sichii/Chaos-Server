#region
using System.Text.RegularExpressions;
#endregion

namespace Chaos.Common.Definitions;

// ReSharper disable once PartialTypeWithSinglePart
internal static partial class RegexCache
{
    /// <summary>
    ///     A regex used to split apart commands into prefix, command, and arguments
    /// </summary>
    [GeneratedRegex(
        """
        "([^"]+)"|([^ ]+)
        """,
        RegexOptions.Compiled)]
    internal static partial Regex CommandSplitRegex { get; }
}