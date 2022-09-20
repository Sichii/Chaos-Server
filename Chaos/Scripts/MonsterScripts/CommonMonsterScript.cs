using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Scripts.MonsterScripts.Components;
using Chaos.Services.Scripting.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

public class CommonMonsterScript : CompositeMonsterScript
{
    private static readonly ICollection<string> ScriptKeys = new[]
    {
        GetScriptKey(typeof(AggroTargetingScript)),
        GetScriptKey(typeof(CastingScript)),
        GetScriptKey(typeof(AttackingScript)),
        GetScriptKey(typeof(MoveToTargetScript)),
        GetScriptKey(typeof(WanderingScript)),
        GetScriptKey(typeof(DeathScript))
    };

    /// <inheritdoc />
    public CommonMonsterScript(IScriptProvider scriptProvider, Monster subject)
        : base(subject)
    {
        var script = scriptProvider.CreateScript<IMonsterScript, Monster>(ScriptKeys, subject) as CompositeMonsterScript;

        if (script == null)
            throw new InvalidOperationException("Unable to create componentized script");

        foreach (var component in script)
            Add(component);
    }
}