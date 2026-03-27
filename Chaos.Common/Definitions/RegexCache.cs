#region
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

// ReSharper disable ArrangeAttributes
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

    /// <summary>
    ///     A regex used to find structured message templates in a string
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Tested by string.Inject tests")]
    [GeneratedRegex(@"\{\{|\{[^{}]+\}|\}\}")]
    internal static partial Regex StringProcessingRegex { get; }
}