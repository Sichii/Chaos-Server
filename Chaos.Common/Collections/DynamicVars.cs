using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Common.Abstractions;
using Chaos.Common.Converters;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
/// Represents a collection of dynamic variables with JSON elements as values.
/// </summary>
[JsonConverter(typeof(DynamicVarsConverter))]
public sealed class DynamicVars : IEnumerable<KeyValuePair<string, JsonElement>>, IScriptVars
{
    private readonly JsonSerializerOptions JsonOptions;
    private readonly ConcurrentDictionary<string, object?> ValueCache;
    private readonly ConcurrentDictionary<string, JsonElement> Vars;

    /// <summary>
    ///     Initializes a new instance of the DynamicVars class with an optional initial set of key-value pairs.
    /// </summary>
    /// <param name="collection">A collection of key-value pairs to initially populate the collection with</param>
    /// <param name="options">The options to use when deserializing jsonElements</param>
    public DynamicVars(IDictionary<string, JsonElement> collection, JsonSerializerOptions options)
    {
        JsonOptions = options;
        Vars = new ConcurrentDictionary<string, JsonElement>(collection, StringComparer.OrdinalIgnoreCase);
        ValueCache = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Initializes a new instance of the DynamicVars class with an optional initial set of key-value pairs.
    /// </summary>
    /// <param name="collection">A collection of key-value pairs to initially populate the collection with</param>
    /// <param name="valueCache">A collection of values that have already been converted from the vars</param>
    /// <param name="options">The options to use when deserializing jsonElements</param>
    public DynamicVars(IDictionary<string, JsonElement> collection, IDictionary<string, object?> valueCache, JsonSerializerOptions options)
    {
        JsonOptions = options;
        Vars = new ConcurrentDictionary<string, JsonElement>(collection, StringComparer.OrdinalIgnoreCase);
        ValueCache = new ConcurrentDictionary<string, object?>(valueCache, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Initializes a new instance of the DynamicVars class
    /// </summary>
    /// <param name="options">The options to use when deserializing jsonElements</param>
    public DynamicVars(JsonSerializerOptions options)
    {
        JsonOptions = options;
        Vars = new ConcurrentDictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        ValueCache = new ConcurrentDictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public bool ContainsKey(string key) => Vars.ContainsKey(key);

    /// <inheritdoc />
    public T? Get<T>(string key) => Get(typeof(T), key) is T t ? t : default;

    /// <inheritdoc />
    public object? Get(Type type, string key) =>
        ValueCache.GetOrAdd(
            key,
            static (k, v) => GetValue(
                v.Type,
                k,
                v.Lookup,
                v.JsonOptions),
            (Type: type, Lookup: Vars, JsonOptions));

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, JsonElement>> GetEnumerator() => Vars.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public T GetRequired<T>(string key) =>
        (Vars.TryGetValue(key, out var value)
            ? value.Deserialize<T>(JsonOptions)
            : throw new KeyNotFoundException($"Required key \"{key}\" was not found while populating script variables"))
        ?? throw new NullReferenceException(
            $"Required key \"{key}\" was found, but resulted in a default value while populating script variables");

    private static object? GetValue(
        Type type,
        string key,
        IDictionary<string, JsonElement> lookup,
        JsonSerializerOptions jsonOptions
    )
    {
        if (!lookup.TryGetValue(key, out var value))
            return default;

        return value.Deserialize(type, jsonOptions);
    }
}