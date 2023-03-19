using System.Collections.Concurrent;
using Chaos.Common.Abstractions;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
/// </summary>
public sealed class StaticVars : IScriptVars
{
    private readonly ConcurrentDictionary<string, object> Vars;

    /// <summary>
    ///     Initializes a new instance of the StaticVars class with an optional initial set of key-value pairs.
    /// </summary>
    /// <param name="objs"></param>
    public StaticVars(IDictionary<string, object> objs) =>
        Vars = new ConcurrentDictionary<string, object>(objs, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool ContainsKey(string key) => Vars.ContainsKey(key);

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        if (Get(typeof(T), key) is T t)
            return t;

        return default;
    }

    /// <inheritdoc />
    public object? Get(Type type, string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            return null;

        if (value is IConvertible)
            return Convert.ChangeType(value, type);

        return value;
    }

    /// <inheritdoc />
    public T GetRequired<T>(string key)
    {
        if (!Vars.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"Required key \"{key}\" was not found while populating script variables");

        return value switch
        {
            IConvertible => (T)Convert.ChangeType(value, typeof(T)),
            T t          => t,
            _ => throw new NullReferenceException(
                $"Required key \"{key}\" was found, but resulted in a default value while populating script variables")
        };
    }
}