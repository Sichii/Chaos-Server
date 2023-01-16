using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

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

    public ArgumentCollection(IList<string> arguments, string? delimiter = null)
    {
        if (!string.IsNullOrEmpty(delimiter))
            arguments = arguments.SelectMany(str => str.Split(delimiter)).ToList();

        Arguments = arguments.ToList();
    }

    public ArgumentCollection(string arguments, string? delimiter = null) => Arguments =
        !string.IsNullOrEmpty(delimiter) ? arguments.Split(delimiter).ToList() : new List<string> { arguments };

    public ArgumentCollection() => Arguments = new List<string>();

    /// <summary>
    ///     Adds a sequence of strings to the end of the collection. Strings will be split by the given delimiter if one is provided.
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
    ///     Adds a sequence of strings or arguments to the end of the collection. Strings will be split by the given delimiter if one is provided.
    /// </summary>
    /// <param name="arguments">A sequence of string arguments</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    public void Add(string? delimiter = null, params string[] arguments) => Add(arguments, delimiter);

    /// <summary>
    ///     Adds a string or argument to the end of the collection. The string will be split by the given delimiter if one is provided.
    /// </summary>
    /// <param name="argument">A string or argument</param>
    /// <param name="delimiter">The delimiter used to split the strings into arguments</param>
    public void Add(string argument, string? delimiter = null) => Add(delimiter, arguments: argument);

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => Arguments.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => string.Join(" ", Arguments);

    /// <summary>
    ///     Attempts to retreive the argument at the given index and convert it to the specified type
    /// </summary>
    /// <param name="index">The index to fetch the argument from</param>
    /// <param name="value">The argument converted to the specified type</param>
    /// <typeparam name="T">The type to convert the argument to</typeparam>
    /// <returns><c>true</c> if an argument exists at the given index and is convertible to the specified type, otherwise <c>false</c></returns>
    public bool TryGet<T>(int index, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        try
        {
            if (Arguments.Count <= index)
                return false;

            var argument = Arguments[index];
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
    /// <returns><c>true</c> if an argument exists at the next index and is convertible to the specified type, otherwise <c>false</c></returns>
    public bool TryGetNext<T>([MaybeNullWhen(false)] out T value)
    {
        var result = TryGet(Index, out value);

        if (result)
            Index++;

        return result;
    }
}