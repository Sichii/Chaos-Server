using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

public class ArgumentCollection : IEnumerable<string>
{
    private readonly List<string> Arguments;
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

    public void Add(IEnumerable<string> arguments, string? delimiter = null)
    {
        if (!string.IsNullOrEmpty(delimiter))
            arguments = arguments.SelectMany(str => str.Split(delimiter)).ToList();

        Arguments.AddRange(arguments);
    }

    public void Add(string argument, string? delimiter = null)
    {
        if (!string.IsNullOrEmpty(delimiter))
            Arguments.AddRange(argument.Split(delimiter));
        else
            Arguments.Add(argument);
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => Arguments.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => string.Join(" ", Arguments);

    public bool TryGet<T>(int index, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        try
        {
            if (Arguments.Count <= index)
                return false;

            var argument = Arguments[index];
            value = PrimitiveConverter.Convert<T>(argument);

            return value != null;
        } catch
        {
            return false;
        }
    }
}