using Chaos.Core.Utilities;
using Chaos.Objects.Panel;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Scripts.SkillScripts;
using Chaos.Services.Factories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SkillScriptFactory : ISkillScriptFactory
{
    private readonly ILogger Logger;
    private readonly ConcurrentDictionary<string, Type> ScriptTypeCache;

    public SkillScriptFactory(ILogger<SkillScriptFactory> logger)
    {
        ScriptTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;

        LoadScriptTypes();
    }

    public ISkillScript CreateScript(ICollection<string> scriptKeys, Skill source)
    {
        if (!scriptKeys.Any())
            throw new InvalidOperationException($"No script keys specified for skill \"{source.Template.Name}\"");

        var compositeScript = new CompositeSkillScript();

        if (scriptKeys.Count == 1)
        {
            var scriptKey = scriptKeys.First();

            if (!ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                throw new KeyNotFoundException($"Skill script key \"{scriptKey}\" was not found");

            var instance = InstanceFactory.CreateInstance(scriptType, source);

            if (instance is not ISkillScript skillScript)
                throw new InvalidCastException($"Script obtained from key \"{scriptKey}\" is not a valid skill script");

            compositeScript.Add(skillScript);
        } else
            foreach (var scriptKey in scriptKeys)
                if (ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                {
                    var instance = InstanceFactory.CreateInstance(scriptType, source);

                    if (instance is not ISkillScript skillScript)
                    {
                        Logger.LogError("Script obtained from key \"{ScriptKey}\" is not a valid skill script", scriptKey);

                        continue;
                    }

                    compositeScript.Add(skillScript);
                }

        if (!compositeScript.Any())
            Logger.LogError("Composite item script for skill \"{SkillName}\" has no components", source.Template.Name);

        return compositeScript;
    }

    private void LoadScriptTypes()
    {
        var types = TypeLoader.LoadTypes<ISkillScript>();

        foreach (var type in types)
        {
            var scriptKey = ScriptBase.GetScriptKey(type);
            ScriptTypeCache.TryAdd(scriptKey, type);
            Logger.LogTrace("Loaded skill script with key {ScriptKey} for type {Type}", scriptKey, type.Name);
        }

        Logger.LogInformation("{Count} skill scripts loaded", ScriptTypeCache.Count);
    }
}