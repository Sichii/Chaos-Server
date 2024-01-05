namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Span{T}" />
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    ///     Determines if a span of chars contains a specific string in a case insensitive manner
    /// </summary>
    /// <param name="span">
    ///     The span to check for the string
    /// </param>
    /// <param name="str">
    ///     The string to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the string was found in the span regardless of case, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool ContainsI(this ReadOnlySpan<char> span, ReadOnlySpan<char> str)
        => span.Contains(str, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines if a span of chars ends with a specific string in a case insensitive manner
    /// </summary>
    /// <param name="span">
    ///     The span to check for the string
    /// </param>
    /// <param name="str">
    ///     The string to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the string was at the end of the span regardless of case, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool EndsWithI(this ReadOnlySpan<char> span, ReadOnlySpan<char> str)
        => span.EndsWith(str, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    ///     Determines if a span of chars starts with a specific string in a case insensitive manner
    /// </summary>
    /// <param name="span">
    ///     The span to check for the string
    /// </param>
    /// <param name="str">
    ///     The string to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the string was found at the beginning of the span regardless of case, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool StartsWithI(this ReadOnlySpan<char> span, ReadOnlySpan<char> str)
        => span.StartsWith(str, StringComparison.OrdinalIgnoreCase);
}