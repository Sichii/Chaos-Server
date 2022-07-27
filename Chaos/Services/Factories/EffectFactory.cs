using Chaos.Core.Utilities;
using Chaos.Effects.Abstractions;
using Chaos.Effects.Interfaces;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Factories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class EffectFactory : IEffectFactory
{
    private readonly ILogger Logger;
    private readonly ConcurrentDictionary<string, Type> EffectTypeCache;

    public EffectFactory(ILogger<EffectFactory> logger)
    {
        EffectTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        
        LoadEffectTypes();
    }

    public IEffect CreateEffect(string effectName, Creature source, Creature target)
    {
        if (!EffectTypeCache.TryGetValue(effectName, out var effectType))
            throw new KeyNotFoundException($"Effect key \"{effectName}\" was not found");

        var instance = InstanceFactory.CreateInstance(effectType, source, target);

        if (instance is not IEffect effect)
            throw new InvalidCastException($"Object obtained from key \"{effectName}\" is not a valid effect");

        return effect;
    }

    private void LoadEffectTypes()
    {
        var types = TypeLoader.LoadTypes<IEffect>();

        foreach (var type in types)
        {
            var effectKey = EffectBase.GetEffectKey(type);
            EffectTypeCache.TryAdd(effectKey, type);
            Logger.LogTrace("Loaded effect type with key {EffectKey} for type {Type}", effectKey, type.Name);
        }

        Logger.LogInformation("{Count} effects loaded", EffectTypeCache.Count);
    }
}