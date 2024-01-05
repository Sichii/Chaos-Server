using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;
using Chaos.Common.Converters;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
/// </summary>
public class StaticVars : IScriptVars
{
    /// <summary>
    ///     Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    /// </param>
    [ExcludeFromCodeCoverage(Justification = "Nothing test, just a wrapper")]
    public object this[string key]
    {
        get => Vars[key];
        set => Vars[key] = value;
    }

    /// <summary>
    ///     Stores key-value pairs of key to variable-values
    /// </summary>
    protected ConcurrentDictionary<string, object> Vars { get; }

    /// <summary>
    ///     Initializes a new instance of the StaticVars class with an optional initial set of key-value pairs.
    /// </summary>
    /// <param name="objs">
    /// </param>
    public StaticVars(IDictionary<string, object>? objs = null)
        => Vars = new ConcurrentDictionary<string, object>(objs ?? new Dictionary<string, object>(), StringComparer.OrdinalIgnoreCase);

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

        return value switch
        {
            IConvertible => PrimitiveConverter.Convert(type, value),
            _            => value
        };
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

    /// <summary>
    ///     Sets the value associated with the specified key.
    /// </summary>
    public void Set(string key, object value) => Vars[key] = value;
}