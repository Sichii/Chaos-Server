using Chaos.Extensions.Common;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class EffectFactory : IEffectFactory
{
    private readonly ConcurrentDictionary<string, Type> EffectTypeCache;
    private readonly ILogger<EffectFactory> Logger;
    private readonly IServiceProvider Provider;

    public EffectFactory(ILogger<EffectFactory> logger, IServiceProvider provider)
    {
        EffectTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        Provider = provider;

        LoadEffectTypes();
    }

    public IEffect Create(string effectKey)
    {
        if (!EffectTypeCache.TryGetValue(effectKey, out var effectType))
            throw new KeyNotFoundException($"Effect key \"{effectKey}\" was not found");

        var instance = ActivatorUtilities.CreateInstance(Provider, effectType);

        if (instance is not IEffect effect)
            throw new InvalidCastException($"Object obtained from key \"{effectKey}\" is not a valid effect");

        Logger.LogTrace("Created effect \"{EffectName}\"", effect.Name);

        return effect;
    }

    private void LoadEffectTypes()
    {
        var types = typeof(IEffect).LoadImplementations();

        foreach (var type in types)
        {
            var effectKey = EffectBase.GetEffectKey(type);
            EffectTypeCache.TryAdd(effectKey, type);
            Logger.LogTrace("Loaded effect type with key {EffectKey} for type {Type}", effectKey, type.Name);
        }

        Logger.LogInformation("{Count} effects loaded", EffectTypeCache.Count);
    }
}