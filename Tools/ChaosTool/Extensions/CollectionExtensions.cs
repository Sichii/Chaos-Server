using ChaosTool.Model;

namespace ChaosTool.Extensions;

internal static class CollectionExtensions
{
    internal static IEnumerable<BindableString> ToBindableStrings(this IEnumerable<string>? strings) =>
        strings?.Select(@string => (BindableString)@string) ?? Enumerable.Empty<BindableString>();

    internal static IEnumerable<string> ToStrings(this IEnumerable<BindableString>? bindableStrings) =>
        bindableStrings?.Select(bindableString => (string)bindableString) ?? Enumerable.Empty<string>();
}