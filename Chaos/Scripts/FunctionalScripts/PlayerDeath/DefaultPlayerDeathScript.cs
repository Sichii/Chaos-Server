using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;

namespace Chaos.Scripts.FunctionalScripts.PlayerDeath;

public class DefaultPlayerDeathScript : ScriptBase, IPlayerDeathScript
{
    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultPlayerDeathScript));

    /// <inheritdoc />
    public static IPlayerDeathScript Create() => FunctionalScriptRegistry.Instance.Get<IPlayerDeathScript>(Key);

    /// <inheritdoc />
    public virtual void OnDeath(Aisling aisling, Creature killedBy) => aisling.IsDead = true;
}