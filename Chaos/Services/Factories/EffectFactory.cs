using System.Collections.Frozen;
using Chaos.Extensions.Common;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class EffectFactory : IEffectFactory
{
    private readonly FrozenDictionary<string, Type> EffectTypeCache;
    private readonly ILogger<EffectFactory> Logger;
    private readonly IServiceProvider Provider;

    public EffectFactory(ILogger<EffectFactory> logger, IServiceProvider provider)
    {
        Logger = logger;
        Provider = provider;

        EffectTypeCache = LoadEffectTypes()
            .ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    public IEffect Create(string effectKey)
    {
        if (!EffectTypeCache.TryGetValue(effectKey, out var effectType))
            throw new KeyNotFoundException($"Effect key \"{effectKey}\" was not found");

        var instance = ActivatorUtilities.CreateInstance(Provider, effectType);

        if (instance is not IEffect effect)
            throw new InvalidCastException($"Object obtained from key \"{effectKey}\" is not a valid effect");

        return effect;
    }

    private Dictionary<string, Type> LoadEffectTypes()
    {
        var ret = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        var types = typeof(IEffect).LoadImplementations();

        foreach (var type in types)
        {
            var effectKey = EffectBase.GetEffectKey(type);
            ret.TryAdd(effectKey, type);

            Logger.WithTopics(Topics.Entities.Effect, Topics.Actions.Load)
                  .LogTrace("Loaded effect type with key {@EffectKey} for type {@Type}", effectKey, type.Name);
        }

        Logger.WithTopics(Topics.Entities.Effect, Topics.Actions.Load)
              .LogInformation("{Count} effects loaded", ret.Count);

        return ret;
    }
}