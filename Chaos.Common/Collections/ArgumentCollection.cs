using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Converters;

namespace Chaos.Common.Collections;

public class ArgumentCollection : IEnumerable<string>
{
    private readonly IList<string> Arguments;
    public ArgumentCollection(IList<string> arguments) => Arguments = arguments;

    public bool TryGet<T>(int index, [MaybeNullWhen(false)] out T value)
    {
        value = default;
        
        if (Arguments.Count <= index)
            return false;
        
        var argument = Arguments[index];
        value = PrimitiveConverter.Convert<T>(argument);

        return value != null;
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator() => Arguments.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public override string ToString() => string.Join(" ", Arguments);
}