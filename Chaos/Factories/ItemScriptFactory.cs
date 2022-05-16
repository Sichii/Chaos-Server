using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Scripts.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Scripts.ItemScripts;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class ItemScriptFactory : IItemScriptFactory
{
    private readonly ILogger Logger;
    private readonly ConcurrentDictionary<string, Type> ScriptTypeCache;

    public ItemScriptFactory(ILogger<ItemScriptFactory> logger)
    {
        ScriptTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
        LoadScriptTypes();
    }

    public IItemScript CreateScript(ICollection<string> scriptKeys, Item source)
    {
        if (!scriptKeys.Any())
            throw new InvalidOperationException($"No script keys specified for item \"{source.DisplayName}\"");

        var compositeScript = new CompositeItemScript(source);

        if (scriptKeys.Count == 1)
        {
            var scriptKey = scriptKeys.First();

            if (!ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                throw new KeyNotFoundException($"Item script key \"{scriptKey}\" was not found");

            var instance = InstanceFactory.CreateInstance(scriptType, source);

            if (instance is not IItemScript itemScript)
                throw new InvalidCastException($"Script obtained from key \"{scriptKey}\" is not a valid item script");

            compositeScript.Add(itemScript);
        } else
            foreach (var scriptKey in scriptKeys)
                if (ScriptTypeCache.TryGetValue(scriptKey, out var scriptType))
                {
                    var instance = InstanceFactory.CreateInstance(scriptType, source);

                    if (instance is not IItemScript itemScript)
                    {
                        Logger.LogError("Script obtained from key \"{ScriptKey}\" is not a valid item script", scriptKey);

                        continue;
                    }

                    compositeScript.Add(itemScript);
                }

        if (!compositeScript.Any())
            Logger.LogError("Composite item script for item \"{ItemName}\" has no components", source.DisplayName);

        return compositeScript;
    }

    private void LoadScriptTypes()
    {
        var types = TypeLoader.LoadTypes<IItemScript>();

        //add type to dictionary
        foreach (var type in types)
        {
            var scriptKey = ScriptBase.GetScriptKey(type);
            ScriptTypeCache.TryAdd(scriptKey, type);
            Logger.LogTrace("Loaded item script with key {ScriptKey} for type {Type}", scriptKey, type.Name);
        }

        Logger.LogInformation("{Count} item scripts loaded", ScriptTypeCache.Count);
    }
}