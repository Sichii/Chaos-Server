using Chaos.Scripting.Abstractions;

namespace Chaos.Extensions;

public static class ScriptExtensions
{
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