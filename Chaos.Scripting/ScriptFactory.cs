using System.Collections.Concurrent;
using Chaos.Extensions.Common;
using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Scripting;

/// <summary>
///     A factory object that generates <see cref="Chaos.Scripting.Abstractions.IScript" />s
/// </summary>
/// <typeparam name="TScript">A type of script</typeparam>
/// <typeparam name="TScripted">A type of scripted object</typeparam>
/// <remarks>
///     This object requires that any given <see cref="Chaos.Scripting.Abstractions.IScript" /> type has an implemented
///     <see cref="Chaos.Scripting.Abstractions.ICompositeScript{TScript}" /> type.
///     This script factory will utilize that composite script to compose multiple scripts into one. The script returned by this factory will
///     always be the <see cref="Chaos.Scripting.Abstractions.ICompositeScript{TScript}" /> implementation, and it will contain all of scripts
///     generated from the keys that
///     are supplied.
/// </remarks>
public sealed class ScriptFactory<TScript, TScripted> : IScriptFactory<TScript, TScripted> where TScript: IScript
                                                                                           where TScripted: IScripted
{
    private readonly Type CompositeType;
    private readonly ILogger<ScriptFactory<TScript, TScripted>> Logger;
    private readonly ConcurrentDictionary<string, Type> ScriptTypeCache;
    private readonly IServiceProvider ServiceProvider;
    private readonly string TypeName;

    public ScriptFactory(ILogger<ScriptFactory<TScript, TScripted>> logger, IServiceProvider serviceProvider)
    {
        ScriptTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        ServiceProvider = serviceProvider;
        TypeName = typeof(TScript).Name;

        LoadScriptTypes();

        CompositeType = ScriptTypeCache.FirstOrDefault(x => x.Key.StartsWithI("composite"))!.Value;
    }

    /// <inheritdoc />
    public TScript CreateScript(ICollection<string> scriptKeys, TScripted subject)
    {
        var composite = (ICompositeScript<TScript>)Activator.CreateInstance(CompositeType)!;

        if (scriptKeys.Count == 1)
        {
            var scriptKey = scriptKeys.First();

            if (!ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                throw new InvalidOperationException($"Script type {scriptKey} not found");

            var instance = ActivatorUtilities.CreateInstance(ServiceProvider, scriptType, subject);

            if (instance is not TScript tScript)
                throw new InvalidCastException($"Script obtained from key \"{scriptKey}\" is not of type {TypeName}");

            composite.Add(tScript);
        } else
            foreach (var scriptKey in scriptKeys)
                if (ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                {
                    var instance = ActivatorUtilities.CreateInstance(ServiceProvider, scriptType, subject);

                    if (instance is not TScript tScript)
                    {
                        Logger.LogError("Script obtained from key \"{ScriptKey}\" is not of type {TypeName}", scriptKey, TypeName);

                        continue;
                    }

                    composite.Add(tScript);
                }

        return (TScript)composite;
    }

    /// <summary>
    ///     Loads all script types that implement the type this factory is for
    /// </summary>
    private void LoadScriptTypes()
    {
        var scriptType = typeof(TScript);
        var types = scriptType.LoadImplementations();

        foreach (var type in types)
        {
            var scriptKey = ScriptBase.GetScriptKey(type);
            ScriptTypeCache[scriptKey] = type;

            Logger.LogDebug(
                "Loaded {TScriptName} with key {ScriptKey} for type {Type}",
                TypeName,
                scriptKey,
                type.Name);
        }

        Logger.LogInformation("{Count} {TScriptName}s loaded", ScriptTypeCache.Count, TypeName);
    }
}