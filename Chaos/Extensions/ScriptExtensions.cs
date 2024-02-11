using Chaos.Scripting.Abstractions;

namespace Chaos.Extensions;

public static class ScriptExtensions
{
    public static void AddScript<TScripted, TScriptBase>(
        this TScripted scripted,
        Type scriptTypeToAdd,
        IScriptProvider scriptProvider) where TScripted: IScripted<TScriptBase>
                                                 where TScriptBase: IScript
    {
        var scriptKey = ScriptBase.GetScriptKey(scriptTypeToAdd);

        var script = scriptProvider.CreateScript<TScriptBase, TScripted>(
            new[]
            {
                scriptKey
            },
            scripted);
        var scripts = script as IEnumerable<TScriptBase>;
        var composite = (ICompositeScript<TScriptBase>)scripted.Script;

        if (scripts is not null)
            foreach (var createdScript in scripts)
                composite.Add(createdScript);
        else
            composite.Add(script);

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

        return Enumerable.Empty<TScript>();
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

    public static void RemoveScript<TBaseScript, TScriptToRemove>(this IScripted<TBaseScript> scripted) where TBaseScript: IScript
        where TScriptToRemove: TBaseScript
    {
        if (!scripted.Script.Is<TScriptToRemove>(out var scriptToRemove))
            return;

        if (scripted.Script is ICompositeScript<TBaseScript> composite)
            composite.Remove(scriptToRemove);
    }
}