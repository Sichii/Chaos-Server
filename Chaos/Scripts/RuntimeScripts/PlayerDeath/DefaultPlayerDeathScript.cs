using Chaos.Objects.World;
using Chaos.Scripts.RuntimeScripts.Abstractions;

namespace Chaos.Scripts.RuntimeScripts.PlayerDeath;

public class DefaultPlayerDeathScript : IPlayerDeathScript
{
    /// <inheritdoc />
    public void OnDeath(Aisling aisling) => aisling.IsDead = true;
}