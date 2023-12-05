using System.Text.RegularExpressions;

namespace Chaos.Common.Definitions;

internal static partial class RegexCache
{
    /// <summary>
    ///     A regex used to split apart commands into prefix, command, and arguments
    /// </summary>
    internal static readonly Regex COMMAND_SPLIT_REGEX = GenerateCommandSplitRegex();

    [GeneratedRegex(
        """
        "([^"]+)"|([^ ]+)
        """,
        RegexOptions.Compiled)]
    private static partial Regex GenerateCommandSplitRegex();
}