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
        var composite = (ICompositeScript<TScript>)scripted.Script;
        composite.Add(script);
        scripted.ScriptKeys.Add(scriptKey);

        if (scripted is PanelEntityBase panelEntity && panelToUpdate is not null)
            panelToUpdate.Update(panelEntity.Slot);
    }

    public static TScript? As<TScript>(this IScript script) =>
        script switch
        {
            TScript typedScript              => typedScript,
            ICompositeScript compositeScript => compositeScript.GetComponent<TScript>(),
            _                                => default
        };

    public static IEnumerable<TScript> GetComponents<TScript>(this IScript script)
    {
        if (script is ICompositeScript compositeScript)
            return compositeScript.GetComponents<TScript>();

        return Enumerable.Empty<TScript>();
    }

    public static bool Is<TScript>(this IScript script, [MaybeNullWhen(false)] out TScript outScript)
    {
        outScript = script.As<TScript>();

        return outScript is not null;
    }
}