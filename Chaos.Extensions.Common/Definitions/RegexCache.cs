using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Definitions;

internal static partial class RegexCache
{
    /// <summary>
    ///     A regex used to find structured message templates in a string
    /// </summary>
    internal static readonly Regex STRING_PROCESSING_REGEX = GenerateStringProcessingRegex();

    [ExcludeFromCodeCoverage(Justification = "Tested by string.Inject tests")]
    [GeneratedRegex(@"\{\{|\{[^{}]+\}|\}\}")]
    internal static partial Regex GenerateStringProcessingRegex();
}