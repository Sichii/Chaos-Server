// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Span{T}" />
/// </summary>
public static class SpanExtensions
{
    /// <param name="span">
    ///     The span to check for the string
    /// </param>
    extension(ReadOnlySpan<char> span)
    {
        /// <summary>
        ///     Determines if a span of chars contains a specific string in a case insensitive manner
        /// </summary>
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
        public bool ContainsI(ReadOnlySpan<char> str) => span.Contains(str, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Determines if a span of chars ends with a specific string in a case insensitive manner
        /// </summary>
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
        public bool EndsWithI(ReadOnlySpan<char> str) => span.EndsWith(str, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Determines if a span of chars starts with a specific string in a case insensitive manner
        /// </summary>
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
        public bool StartsWithI(ReadOnlySpan<char> str) => span.StartsWith(str, StringComparison.OrdinalIgnoreCase);
    }
}