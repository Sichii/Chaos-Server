using System.Collections.Concurrent;
using Chaos.Common.Abstractions;

namespace Chaos.Common.Collections;

public sealed class StaticVars : IScriptVars
{
    private readonly ConcurrentDictionary<string, object> Vars;

    public StaticVars(IDictionary<string, object> objs) =>
        Vars = new ConcurrentDictionary<string, object>(objs, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public T? Get<T>(string key) => (T?)Get(typeof(T), key);

    /// <inheritdoc />
    public object? Get(Type type, string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            return null;

        if (value is IConvertible)
            return Convert.ChangeType(value, type);

        return value;
    }
}