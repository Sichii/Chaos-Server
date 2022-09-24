using System.Collections.Concurrent;
using Chaos.Core.Utilities;
using Chaos.Extensions.Common;
using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting;

public class ScriptFactory<TScript, TSource> : IScriptFactory<TScript, TSource> where TScript: IScript
                                                                                where TSource: IScripted
{
    private readonly Type CompositeType;
    private readonly ILogger Logger;
    private readonly ConcurrentDictionary<string, Type> ScriptTypeCache;
    private readonly IServiceProvider ServiceProvider;
    private readonly string TypeName;

    public ScriptFactory(ILogger<ScriptFactory<TScript, TSource>> logger, IServiceProvider serviceProvider)
    {
        ScriptTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        ServiceProvider = serviceProvider;
        TypeName = typeof(TScript).Name;

        LoadScriptTypes();

        CompositeType = ScriptTypeCache.FirstOrDefault(x => x.Key.ContainsI("composite"))!.Value;
    }

    /// <inheritdoc />
    public TScript CreateScript(ICollection<string> scriptKeys, TSource source)
    {
        var composite = (ICompositeScript<TScript>)Activator.CreateInstance(CompositeType, source)!;

        if (scriptKeys.Count == 1)
        {
            var scriptKey = scriptKeys.First();

            if (!ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                throw new InvalidOperationException($"Script type {scriptKey} not found");

            var instance = ActivatorUtilities.CreateInstance(ServiceProvider, scriptType, source);

            if (instance is not TScript tScript)
                throw new InvalidCastException($"Script obtained from key \"{scriptKey}\" is not of type {TypeName}");

            composite.Add(tScript);
        } else
            foreach (var scriptKey in scriptKeys)
                if (ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                {
                    var instance = ActivatorUtilities.CreateInstance(ServiceProvider, scriptType, source);

                    if (instance is not TScript tScript)
                    {
                        Logger.LogError("Script obtained from key \"{ScriptKey}\" is not of type {TypeName}", scriptKey, TypeName);

                        continue;
                    }

                    composite.Add(tScript);
                }

        return (TScript)composite;
    }

    private void LoadScriptTypes()
    {
        var types = TypeLoader.LoadImplementations<TScript>();

        foreach (var type in types)
        {
            var scriptKey = ScriptBase.GetScriptKey(type);
            ScriptTypeCache[scriptKey] = type;

            Logger.LogTrace(
                "Loaded {TScriptName} with key {ScriptKey} for type {Type}",
                TypeName,
                scriptKey,
                type.Name);
        }

        Logger.LogInformation("{Count} {TScriptName}s loaded", ScriptTypeCache.Count, TypeName);
    }
}