using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Chaos.Common.Converters;
using Chaos.Common.Definitions;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
///     A collection that stores string arguments and makes them more accessible
/// </summary>
public sealed class ArgumentCollection : IEnumerable<string>
{
    private readonly List<string> Arguments;
    private int Index;

    /// <summary>
    ///     The number of arguments in the collection
    /// </summary>
    public int Count => Arguments.Count;

    /// <summary>
    ///     Creates an <see cref="ArgumentCollection" /> from a sequence of strings. Strings will be split by the given
    ///     delimiter if one is
    ///     provided.
    /// </summary>
    /// <param name="arguments">A sequence of argument strings</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    public ArgumentCollection(IEnumerable<string>? arguments, string? delimiter = null)
    {
        arguments ??= Enumerable.Empty<string>();

        if (!string.IsNullOrEmpty(delimiter))
            arguments = arguments.SelectMany(str => str.Split(delimiter)).ToList();

        Arguments = arguments.ToList();
    }

    /// <summary>
    ///     Creates an <see cref="ArgumentCollection" /> from a string. String will be parsed for arguments using the " " as a
    ///     delimiter, but
    ///     keeping double quoted strings intact.
    /// </summary>
    /// <param name="argumentStr">A string containing arguments</param>
    public ArgumentCollection(string argumentStr)
    {
        Arguments = new List<string>();

        foreach (var match in RegexCache.COMMAND_SPLIT_REGEX.Matches(argumentStr).OfType<Match>())
        {
            if (!match.Success)
                continue;

            var grp = match.Groups[1].Value;
            Arguments.Add(!string.IsNullOrEmpty(grp) ? grp : match.Groups[2].Value);
        }
    }

    /// <summary>
    ///     Creates an <see cref="ArgumentCollection" /> from a string. String will be parsed for arguments using the given
    ///     delimiter.
    /// </summary>
    /// <param name="argumentStr">A string containing arguments</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    public ArgumentCollection(string argumentStr, string delimiter) => Arguments = argumentStr.Split(delimiter).ToList();

    /// <summary>
    ///     Creates an empty <see cref="ArgumentCollection" />
    /// </summary>
    public ArgumentCollection() => Arguments = new List<string>();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => Arguments.GetEnumerator();

    /// <summary>
    ///     Adds a sequence of strings to the end of the collection. Strings will be split by the given delimiter if one is
    ///     provided.
    /// </summary>
    /// <param name="arguments">A sequence of string arguments</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    public void Add(IEnumerable<string> arguments, string? delimiter = null)
    {
        if (!string.IsNullOrEmpty(delimiter))
            arguments = arguments.SelectMany(str => str.Split(delimiter));

        Arguments.AddRange(arguments);
    }

    /// <summary>
    ///     Adds a string or argument to the end of the collection. The string will be split by the given delimiter if one is
    ///     provided.
    /// </summary>
    /// <param name="argument">A string containing arguments</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    [ExcludeFromCodeCoverage(Justification = "Nothing to test, just a shorthand")]
    public void Add(string argument, string? delimiter) => Add(new[] { argument }, delimiter);

    /// <summary>
    ///     Adds a string or argument to the end of the collection. The string will be parsed for arguments using the " " as a
    ///     delimiter, but
    ///     keeping double quoted strings intact.
    /// </summary>
    /// <param name="argument">A string containing arguments</param>
    public void Add(string argument)
    {
        foreach (var match in RegexCache.COMMAND_SPLIT_REGEX.Matches(argument).OfType<Match>())
        {
            if (!match.Success)
                continue;

            var grp = match.Groups[1].Value;
            Arguments.Add(!string.IsNullOrEmpty(grp) ? grp : match.Groups[2].Value);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var arg in Arguments)
        {
            sb.Append('"');
            sb.Append(arg);
            sb.Append('"');
            sb.Append(' ');
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Attempts to retreive the argument at the given index and convert it to the specified type
    /// </summary>
    /// <param name="index">The index to fetch the argument from</param>
    /// <param name="value">The argument converted to the specified type</param>
    /// <typeparam name="T">The type to convert the argument to</typeparam>
    /// <returns>
    ///     <c>true</c> if an argument exists at the given index and is convertible to the specified type, otherwise
    ///     <c>false</c>
    /// </returns>
    public bool TryGet<T>(int index, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        try
        {
            if (Arguments.Count <= index)
                return false;

            var argument = Arguments[index];

            if (typeof(T) == typeof(ArgumentCollection))
                value = (T)(object)new ArgumentCollection(argument);
            else
                value = PrimitiveConverter.Convert<T>(argument);

            // ReSharper disable once CompareNonConstrainedGenericWithNull
            return value != null;
        } catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Attempts to retreive the argument at the next index and convert it to the specified type
    /// </summary>
    /// <param name="value">The argument converted to the specified type</param>
    /// <typeparam name="T">The type to convert the argument to</typeparam>
    /// <returns>
    ///     <c>true</c> if an argument exists at the next index and is convertible to the specified type, otherwise
    ///     <c>false</c>
    /// </returns>
    public bool TryGetNext<T>([MaybeNullWhen(false)] out T value)
    {
        var result = TryGet(Index, out value);

        if (result)
            Index++;

        return result;
    }
}