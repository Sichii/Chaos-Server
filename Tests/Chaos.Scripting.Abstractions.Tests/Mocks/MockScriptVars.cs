using Chaos.Common.Abstractions;

namespace Chaos.Scripting.Abstractions.Tests.Mocks;

public sealed class MockScriptVars : IScriptVars
{
    private readonly Dictionary<string, object> Values = new();

    public bool ContainsKey(string key) => Values.ContainsKey(key);

    public T? Get<T>(string key)
    {
        if (Values.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    public object? Get(Type type, string key)
    {
        if (Values.TryGetValue(key, out var value) && type.IsInstanceOfType(value))
            return value;

        return null;
    }

    public T GetRequired<T>(string key)
    {
        if (Values.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        throw new Exception($"Key '{key}' not found.");
    }

    public void Set<T>(T value, string key) => Values[key] = value!;
}