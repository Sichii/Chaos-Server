using Chaos.Scripts.RuntimeScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts.PlayerDeath;

namespace Chaos.Scripts.RuntimeScripts;

public static class PlayerDeathScripts
{
    public static IPlayerDeathScript Default { get; } = new DefaultPlayerDeathScript();
}