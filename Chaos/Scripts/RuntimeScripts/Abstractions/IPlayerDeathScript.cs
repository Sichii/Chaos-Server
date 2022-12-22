using Chaos.Objects.World;

namespace Chaos.Scripts.RuntimeScripts.Abstractions;

public interface IPlayerDeathScript
{
    void OnDeath(Aisling aisling);
}