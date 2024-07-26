using Chaos.Common.Comparers;
using Chaos.Extensions.Common;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Extensions;

public static class ScriptExtensions
{
    public static void AddScript<TScript>(this IScripted<IScript> scripted)
    {
        var scriptType = typeof(TScript);
        var scriptKey = ScriptBase.GetScriptKey(scriptType);

        var scriptedType = scripted.GetType();

        var baseScriptType = scriptedType.ExtractGenericInterfaces(typeof(IScripted<>))
                                         .OrderBy(MostDerivedTypeComparer.Instance)
                                         .First()
                                         .GetGenericArguments()
                                         .Single();

        var scriptProvider = AppContext.ScriptProvider;

        var method = scriptProvider.GetType()
                                   .GetGenericMethod(
                                       nameof(IScriptProvider.CreateScript),
                                       [
                                           baseScriptType,
                                           scriptedType
                                       ]);

        var resultCompositeScript = (IEnumerable<IScript>)method!.Invoke(
            scriptProvider,
            [
                new[]
                {
                    scriptKey
                },
                scripted
            ])!;

        var targetCompositeScript = (ICompositeScript)scripted.Script;

        foreach (var script in resultCompositeScript)
            targetCompositeScript.Add(script);

        scripted.ScriptKeys.Add(scriptKey);
    }

    public static TScript? As<TScript>(this IScript script) where TScript: IScript
        => script switch
        {
            TScript typedScript              => typedScript,
            ICompositeScript compositeScript => compositeScript.GetScript<TScript>(),
            _                                => default
        };

    public static IEnumerable<TScript> GetScripts<TScript>(this IScript script) where TScript: IScript
    {
        if (script is ICompositeScript compositeScript)
            return compositeScript.GetScripts<TScript>();

        return [];
    }

    public static bool Is<TScript>(this IScript script) where TScript: IScript
    {
        var outScript = script.As<TScript>();

        return outScript is not null;
    }

    public static bool Is<TScript>(this IScript script, [MaybeNullWhen(false)] out TScript outScript) where TScript: IScript
    {
        outScript = script.As<TScript>();

        return outScript is not null;
    }

    public static void RemoveScript<TScriptToRemove>(this IScripted<IScript> scripted) where TScriptToRemove: IScript
    {
        if (!scripted.Script.Is<TScriptToRemove>(out var scriptToRemove))
            return;

        if (scripted.Script is ICompositeScript composite)
            composite.Remove(scriptToRemove);
    }
    
    public static ISimpleCache GetCacheProvider(this IScript script) 
    {
        return AppContext.CacheProvider;
    }
}