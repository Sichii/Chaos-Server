using System.Text.RegularExpressions;

namespace Chaos.Extensions.Common.Definitions;

internal static partial class RegexCache
{
    /// <summary>
    ///     A regex used to find structured message templates in a string
    /// </summary>
    internal static readonly Regex STRING_PROCESSING_REGEX = GenerateStringProcessingRegex();

    [GeneratedRegex(@"\{\{|\{[^{}]+\}|\}\}")]
    internal static partial Regex GenerateStringProcessingRegex();
}