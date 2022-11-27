using System.Text.RegularExpressions;

namespace Chaos.Extensions.Common.Definitions;

internal static partial class RegexCache
{
    internal static readonly Regex STRING_PROCESSING_REGEX = GenerateStringProcessingRegex();

    [GeneratedRegex(@"\{\{|\{[^{}]+\}|\}\}")]
    internal static partial Regex GenerateStringProcessingRegex();
}