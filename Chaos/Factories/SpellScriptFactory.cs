using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Scripts.SpellScripts;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class SpellScriptFactory : ISpellScriptFactory
{
    private readonly ILogger Logger;
    private readonly ConcurrentDictionary<string, Type> ScriptTypeCache;

    public SpellScriptFactory(ILogger<SpellScriptFactory> logger)
    {
        ScriptTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        LoadScryptTypes();
    }

    public ISpellScript CreateScript(ICollection<string> scriptKeys, Spell source)
    {
        if (!scriptKeys.Any())
            throw new InvalidOperationException($"No script keys specified for spell \"{source.Template.Name}\"");

        var compositeScript = new CompositeSpellScript();

        if (scriptKeys.Count == 1)
        {
            var scriptKey = scriptKeys.First();

            if (!ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                throw new KeyNotFoundException($"Spell script key \"{scriptKey}\" was not found");

            var instance = InstanceFactory.CreateInstance(scriptType, source);

            if (instance is not ISpellScript spellScript)
                throw new InvalidCastException($"Script obtained from key \"{scriptKey}\" is not a valid spell script");

            compositeScript.Add(spellScript);
        } else
            foreach (var scriptKey in scriptKeys)
                if (ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                {
                    var instance = InstanceFactory.CreateInstance(scriptType, source);

                    if (instance is not ISpellScript spellScript)
                    {
                        Logger.LogError("Script obtained from key \"{ScriptKey}\" is not a valid spell script", scriptKey);

                        continue;
                    }

                    compositeScript.Add(spellScript);
                }

        if (!compositeScript.Any())
            Logger.LogError("Composite item script for spell \"{SpellName}\" has no components", source.Template.Name);

        return compositeScript;
    }

    private void LoadScryptTypes()
    {
        var types = TypeLoader.LoadTypes<ISpellScript>();

        foreach (var type in types)
        {
            var scriptKey = ScriptBase.GetScriptKey(type);
            ScriptTypeCache.TryAdd(scriptKey, type);
            Logger.LogTrace("Loaded spell script with key {ScriptKey} for type {Type}", scriptKey, type.Name);
        }

        Logger.LogInformation("{Count} spell scripts loaded", ScriptTypeCache.Count);
    }
}