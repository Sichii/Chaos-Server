using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Abstractions;

public interface IMapScript : IScript
{
    void OnEntered(Creature creature);
    void OnExiting(Creature creature);
}