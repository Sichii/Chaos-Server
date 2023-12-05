using Chaos.Scripting.FunctionalScripts.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Scripting.FunctionalScripts;

public sealed class FunctionalScriptRegistry : IScriptRegistry
{
    private static readonly ConcurrentDictionary<string, Type> Scripts = new(StringComparer.OrdinalIgnoreCase);
    private readonly IServiceProvider Provider;

    /// <inheritdoc />
    public static IScriptRegistry Instance { get; private set; } = null!;

    public FunctionalScriptRegistry(IServiceProvider provider)
    {
        Provider = provider;

        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Instance ??= this;
    }

    /// <inheritdoc />
    public T Get<T>(string key)
    {
        if (!Scripts.TryGetValue(key, out var type))
            throw new KeyNotFoundException($"Script with key '{key}' not found.");

        return (T)ActivatorUtilities.CreateInstance(Provider, type);
    }

    /// <inheritdoc />
    public void Register(string key, Type type) => Scripts.TryAdd(key, type);
}