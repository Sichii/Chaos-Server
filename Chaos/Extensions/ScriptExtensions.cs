using Chaos.Collections.Abstractions;
using Chaos.Models.Panel.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Extensions;

public static class ScriptExtensions
{
    public static void AddScript<TScripted, TScript>(
        this TScripted scripted,
        Type scriptTypeToAdd,
        IScriptFactory<TScript, TScripted> scriptFactory,
        IPanel<TScripted>? panelToUpdate = null
    ) where TScripted: IScripted<TScript> where TScript: IScript
    {
        var scriptKey = ScriptBase.GetScriptKey(scriptTypeToAdd);
        var script = scriptFactory.CreateScript(new[] { scriptKey }, scripted);
        var scripts = script as IEnumerable<TScript>;
        var composite = (ICompositeScript<TScript>)scripted.Script;

        if (scripts is not null)
            foreach (var createdScript in scripts)
                composite.Add(createdScript);
        else
            composite.Add(script);

        scripted.ScriptKeys.Add(scriptKey);

        if (scripted is PanelEntityBase panelEntity && panelToUpdate is not null)
            panelToUpdate.Update(panelEntity.Slot);
    }

    public static TScript? As<TScript>(this IScript script) where TScript: IScript =>
        script switch
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

    public static void RemoveScript<TBaseScript, TScriptToRemove>(this IScripted<TBaseScript> scripted)
        where TBaseScript: IScript where TScriptToRemove: TBaseScript
    {
        if (!scripted.Script.Is<TScriptToRemove>(out var scriptToRemove))
            return;

        if (scripted.Script is ICompositeScript<TBaseScript> composite)
            composite.Remove(scriptToRemove);
    }
}