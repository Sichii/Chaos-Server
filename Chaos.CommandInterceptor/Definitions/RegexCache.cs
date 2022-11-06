using System.Text.RegularExpressions;

namespace Chaos.CommandInterceptor.Definitions;

internal static partial class RegexCache
{
    internal static readonly Regex COMMAND_SPLIT_REGEX = GenerateCommandSplitRegex();

    [GeneratedRegex("""\"([^"]+)"|([^ ]+)""", RegexOptions.Compiled)]
    private static partial Regex GenerateCommandSplitRegex();
}