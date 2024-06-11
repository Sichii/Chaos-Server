using Chaos.Wpf.Observables;

namespace ChaosTool.Extensions;

internal static class CollectionExtensions
{
    internal static IEnumerable<BindableString> ToBindableStrings(this IEnumerable<string>? strings)
        => strings?.Select(@string => (BindableString)@string) ?? [];

    internal static IEnumerable<string> ToStrings(this IEnumerable<BindableString>? bindableStrings)
        => bindableStrings?.Select(bindableString => (string)bindableString) ?? [];
}